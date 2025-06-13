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


namespace LarrysCards.Cards.BulletMods
{
    class SplitBullets : CustomCard
    {
        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            CardInfo = cardInfo;
            cardInfo.allowMultiple = true;
            gun.damage = 0.9f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            ActivatorMono.Add(player, (obj, i) =>
            {
                MoveTransform moveTransform = obj.GetComponent<MoveTransform>();
                if (moveTransform != null)
                {
                    if (moveTransform.gameObject.GetComponent<SplitShots>())
                    {
                        moveTransform.gameObject.GetComponent<SplitShots>().Explode(2, 45f, destroy: obj.isFinalAction);
                    }
                }
            });

            Type type = typeof(SplitShots);

            float defaultTime = 0.49f;

            if (!SplitShots.time.ContainsKey(player.playerID)) SplitShots.time.Add(player.playerID, defaultTime);
            else SplitShots.time[player.playerID] /= 2f;

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            SplitShots.time[player.playerID] = defaultTime;

            GameObject obj = new GameObject("SplitEffect", type);

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
            return "Split Bullets";
        }
        protected override string GetDescription()
        {
            return "<color=#9aa355>TIMED</color>\n" + "Your bullets split after 0.5s";
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
                    simepleAmount = CardInfoStat.SimpleAmount.Some
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

    public class SplitShots : MonoBehaviour
    {

        public static Dictionary<int, float> time = new Dictionary<int, float>();

        public Player owner;

        private MoveTransform moveTransform;

        bool activated;
        public void Awake()
        {
            if (GetComponent<NoSplitMarker>() != null)
            {
                Destroy(this);
                return;
            }
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<SplitShots>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<SplitShots>().Where(bullet => bullet != this))
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
                Explode(2, 45f);
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
            SimulatedGun sgun = new GameObject("SplitGun").AddComponent<SimulatedGun>();
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

                    // Remove ALL SplitShots components from hierarchy
                    var splits = copy.AddToProjectile.GetComponentsInChildren<SplitShots>(true);
                    foreach (var split in splits)
                    {
                        Destroy(split);
                    }

                    // Add a marker component to prevent re-adding SplitShots
                    copy.AddToProjectile.AddComponent<NoSplitMarker>();
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

    internal class NoSplitMarker : MonoBehaviour
    {
    }
}