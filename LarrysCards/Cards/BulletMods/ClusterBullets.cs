using HarmonyLib;
using LarrysCards.Cards.General;
using Photon.Pun;
using SimulationChamber;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;


namespace LarrysCards.Cards.BulletMods
{
    class ClusterBullets : CustomCard
    {
        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            CardInfo = cardInfo;
            cardInfo.allowMultiple = false;
            gun.damage = 0.9f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            ActivatorMono.Add(player, (obj, i) =>
            {
                MoveTransform moveTransform = obj.GetComponent<MoveTransform>();
                if (moveTransform != null)
                {
                    if (moveTransform.gameObject.GetComponent<ClusterShots>())
                    {
                        moveTransform.gameObject.GetComponent<ClusterShots> ().Explode(8, 360f, 0.3f, destroy: obj.isFinalAction);
                    }
                }
            });

            Type type = typeof(ClusterShots);

            float defaultTime = 0.5f;

            if (!ClusterShots.time.ContainsKey(player.playerID)) ClusterShots.time.Add(player.playerID, defaultTime);
            else ClusterShots.time[player.playerID] /= 2f;

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            ClusterShots.time[player.playerID] = defaultTime;

            GameObject obj = new GameObject("ClusterEffect", type);

            gun.objectsToSpawn = gun.objectsToSpawn.Append(new ObjectsToSpawn
            {
                AddToProjectile = obj
            }).ToArray();
        }




        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Cluster Bullets";
        }
        protected override string GetDescription()
        {
            return "<color=#9aa355>TIMED</color>\n" + "Your bullets explode into smaller bullets after 0.5s";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = false,
                    stat = "DMG",
                    amount = "-10%",
                    simepleAmount = CardInfoStat.SimpleAmount.slightlyLower
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Cluster DMG",
                    amount = "30%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.FirepowerYellow;
        }
        public override string GetModName()
        {
            return LarrysCards.ModInitials;
        }
    }

    public class ClusterShots : MonoBehaviour
    {

        public static Dictionary<int, float> time = new Dictionary<int, float>();

        public Player owner;

        private MoveTransform moveTransform;

        bool activated;
        public void Awake()
        {
            if (GetComponent<NoClusterMarker>() != null)
            {
                Destroy(this);
                return;
            }
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<ClusterShots>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<ClusterShots>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            if (GetComponent<SpawnedAttack>() == null) return;

            owner = GetComponent<SpawnedAttack>().spawner;

            if (owner == null) { this.ExecuteAfterFrames(1, () => { Awake(); }); return; }

            activated = owner.GetComponent<ActivatorMono>().actionsEnabled;

            if (activated) return;

            int ownerID = owner.playerID;

            print(ownerID);

            float newtime = time[ownerID];

            print(newtime);

            moveTransform = GetComponent<MoveTransform>();

            if (moveTransform == null) return;

            this.ExecuteAfterSeconds(newtime / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
            {
                Explode(8, 360f, 0.3f);
            });

        }
        public void Explode(int count, float maxAngle, float dmg = 1f, float range = -1f, bool destroy = true)
        {
            if (GetComponent<SpawnedAttack>() == null) return;
            owner = GetComponent<SpawnedAttack>().spawner;

            if (owner == null) return;

            if (transform == null) return;

            moveTransform = GetComponent<MoveTransform>();

            if (moveTransform == null) return;

            Vector2 vel = new Vector2(0, 2f);

            Gun gun = owner.data.weaponHandler.gun;
            SimulatedGun sgun = new GameObject("ClusterGun").AddComponent<SimulatedGun>();
            sgun.CopyGunStatsExceptActions(gun);
            sgun.shootPosition = transform;

            sgun.objectsToSpawn = sgun.objectsToSpawn.Select(original =>
            {
                ObjectsToSpawn copy = new ObjectsToSpawn()
                {
                    // [Copy all properties as before]
                };

                if (original.AddToProjectile != null)
                {
                    copy.AddToProjectile = Instantiate(original.AddToProjectile);

                    // Remove ALL ClusterShots components from hierarchy
                    var Clusters = copy.AddToProjectile.GetComponentsInChildren<ClusterShots>(true);
                    foreach (var Cluster in Clusters)
                    {
                        Destroy(Cluster);
                    }

                    // Add a marker component to prevent re-adding ClusterShots
                    copy.AddToProjectile.AddComponent<NoClusterMarker>();
                }

                return copy;
            }).ToArray();

            sgun.numberOfProjectiles = 1;
            sgun.damage *= dmg;
            if (range > 0f) sgun.destroyBulletAfter = range;

            for (int i = 0; i < count; i++)
            {
                float angleOffset = Mathf.Lerp(-maxAngle / 2, maxAngle / 2, (float)i / (count - 1));
                Vector2 angle = LarrysCards.RotatedBy(moveTransform.velocity, angleOffset);
                sgun.SimulatedAttack(owner.playerID, transform.position, angle, 1f, 1f);
            }

            // Clean up
            if (destroy) Destroy(transform.root.gameObject);
        }
    }

    internal class NoClusterMarker : MonoBehaviour
    {
    }
}