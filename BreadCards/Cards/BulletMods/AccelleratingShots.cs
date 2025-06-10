using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.BulletMods
{
    class AccelleratingShots : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.projectileSpeed /= 2;
            gun.gravity /= 2;

            Type type = typeof(AccellShots);

            float defaultTime = 1f;

            if (!AccellShots.time.ContainsKey(player.playerID)) AccellShots.time.Add(player.playerID, defaultTime);
            else AccellShots.time[player.playerID] += defaultTime;

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }


            AccellShots.time[player.playerID] = defaultTime;

            GameObject obj = new GameObject("AccellShotsEffect", type);

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
            return "Accellerating Shots";
        }
        protected override string GetDescription()
        {
            return "Your bullets will increase their speed every 0.5s";
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
                    positive = true,
                    stat = "Bullet Speed Increase",
                    amount = "+100%",
                    simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullet Gravity",
                    amount = "-50%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Bullet Speed",
                    amount = "-50%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
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


    public class AccellShots : MonoBehaviour
    {
        public static Dictionary<int, float> time = new Dictionary<int, float>();

        public Player owner;

        private MoveTransform moveTransform;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<AccellShots>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<AccellShots>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }

            this.ExecuteAfterSeconds(0.01f, () =>
            {
                moveTransform = GetComponent<MoveTransform>();
                while (owner == null) { owner = GetComponent<SpawnedAttack>().spawner; }

                if (moveTransform != null)
                {
                    SpeedUp();
                }
            });
        }
        public void Update()
        {
        }

        public void SpeedUp()
        {
            this.ExecuteAfterSeconds(0.5f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
            {
                SpeedUp();
                moveTransform.velocity *= 2f;
            });
        }
    }
}