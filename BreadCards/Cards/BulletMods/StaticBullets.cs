using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.BulletMods
{
    class StaticBullets : CustomCard
    {
        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            CardInfo = cardInfo;
            cardInfo.allowMultiple = true;
            gun.damage = 1.5f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Type type = typeof(StaticShots);

            float defaultTime = 0.7f;

            if (!StaticShots.time.ContainsKey(player.playerID)) StaticShots.time.Add(player.playerID, defaultTime);
            else StaticShots.time[player.playerID] /= 2f;

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            StaticShots.time[player.playerID] = defaultTime;

            GameObject obj = new GameObject("StaticEffect", type);

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
            return "Static Bullets";
        }
        protected override string GetDescription()
        {
            return "Your bullets stop after 0.7s";
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
                    positive = true,
                    stat = "DMG",
                    amount = "+50%",
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
            return BreadCards.ModInitials;
        }
    }

    public class StaticShots : MonoBehaviour
    {

        public static Dictionary<int, float> time = new Dictionary<int, float>();

        public Player owner;

        private MoveTransform moveTransform;
        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<StaticShots>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<StaticShots>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            if (owner == null && GetComponent<SpawnedAttack>() != null) { owner = GetComponent<SpawnedAttack>().spawner; this.ExecuteAfterSeconds(0.01f, () => Awake()); return; }
            this.ExecuteAfterSeconds(0.01f, () =>
            {

                int ownerID = owner.playerID;

                print(ownerID);

                float newtime = time[ownerID];

                print(newtime);

                moveTransform = GetComponent<MoveTransform>();

                if (moveTransform == null) return;

                this.ExecuteAfterSeconds(newtime / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                 
                {
                    moveTransform.velocity *= 0.0001f;
                });
            });

        }
        public void Update()
        {
        }
    }
    public class MonoPlayerInitializer : MonoBehaviour
    {
        public Player player;
    }

}