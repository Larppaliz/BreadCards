using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using SimulationChamber;
using HarmonyLib;
using System;

namespace BreadCards.Cards.BulletMods
{
    class ClusterShots : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Type type = typeof(ClusterEffect);

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

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
            return "Cluster Rounds";
        }
        protected override string GetDescription()
        {
            return "Your bullets will explode into multiple smaller bullets after 1s";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = false,
                    stat = "<color=#ffa648>Cluster</color> DMG",
                    amount = "30%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                                new CardInfoStat()
                {
                    positive = false,
                    stat = "<color=#ffa648>Cluster</color> Range",
                    amount = "15%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.TechWhite;
        }
        public override string GetModName()
        {
            return BreadCards.ModInitials;
        }
    }


    public class ClusterEffect : MonoBehaviour
    {

        public Player owner;

        private MoveTransform moveTransform;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<ClusterEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<ClusterEffect>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            if (owner == null && GetComponent<SpawnedAttack>() != null) { owner = GetComponent<SpawnedAttack>().spawner; this.ExecuteAfterSeconds(0.01f, () => Awake()); return; }

            this.ExecuteAfterSeconds(0.2f, () =>
            {
                moveTransform = GetComponent<MoveTransform>();

                this.ExecuteAfterSeconds(0.8f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                {
                    Explode(moveTransform.velocity);
                });
            });
        }
        public void Explode(Vector2 vel)
        {

                int bulletAmount = UnityEngine.Random.Range(6,12);

                Gun gun = owner.data.weaponHandler.gun;
                SimulatedGun sgun = new GameObject("ClusterGun").AddComponent<SimulatedGun>();

                sgun.CopyGunStatsExceptActions(gun);

                sgun.shootPosition = transform;


                ObjectsToSpawn[] list = new ObjectsToSpawn[0];

                foreach (ObjectsToSpawn obj in gun.objectsToSpawn)
                {
                    if (!obj.AddToProjectile.GetComponent<ClusterEffect>())
                    {
                        list.AddItem(obj);
                    }
                }
                sgun.objectsToSpawn = list;
                sgun.numberOfProjectiles = 1;
                sgun.destroyBulletAfter *= 0.15f;
                sgun.damage = 0.3f;

                float pspd = sgun.projectileSpeed;

                for (int i = 0; i < bulletAmount; i++)
                {
                    Vector2 angle = BreadCards.RotatedBy(vel, 360/bulletAmount * i);

                    sgun.projectileSpeed = pspd * UnityEngine.Random.Range(0.85f, 1.15f);

                    sgun.SimulatedAttack(owner.playerID, transform.position, angle, 1f, 1f);
                }


                foreach (var obj in GetComponentsInChildren<ClusterEffect>().Where(bullet => bullet == this))
                {
                    Destroy(obj.gameObject);
                }
        }
    }
}