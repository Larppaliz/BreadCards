using LarrysCards.Cards.General;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.BulletMods
{
    class ReversingBullets : CustomCard
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
            ActivatorMono.Add(player, (obj, i) =>
            {
                MoveTransform moveTransform = obj.GetComponent<MoveTransform>();
                if (moveTransform != null)
                {
                    moveTransform.velocity *= -1;
                }
            });

            Type type = typeof(ReverseShots);

            float defaultTime = 1.01f;

            if (!ReverseShots.time.ContainsKey(player.playerID)) ReverseShots.time.Add(player.playerID, defaultTime);
            else ReverseShots.time[player.playerID] /= 2f;

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            ReverseShots.time[player.playerID] = defaultTime;

            GameObject obj = new GameObject("ReverseEffect", type);

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
            return "Reversing Bullets";
        }
        protected override string GetDescription()
        {
            return "<color=#9aa355>TIMED</color>\n" + "Your bullets reverse after 1s";
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
            return LarrysCards.ModInitials;
        }
    }

    public class ReverseShots : MonoBehaviour
    {

        public static Dictionary<int, float> time = new Dictionary<int, float>();

        public Player owner;

        private MoveTransform moveTransform;

        bool activated;
        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<ReverseShots>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<ReverseShots>().Where(bullet => bullet != this))
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
                moveTransform.velocity *= -1;
            });

        }
    }
}