using ClassesManagerReborn.Util;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.Classes.Shulker
{
    class LessTPDelay : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;
        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = "Zip " + ShulkerClass.name;
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            ShulkerData sData = ShulkerHoming.stats[player.playerID];

            sData.TPdelay *= 0.65f;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            ShulkerData sData = ShulkerHoming.stats[player.playerID];

            sData.TPdelay /= 0.65f;
        }
        protected override string GetTitle()
        {
            return "Fasterry Zippingy Shulkerty";
        }
        protected override string GetDescription()
        {
            return "Decreases your shulker shots zipping delay";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.ShulkerArt;
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
                    stat = "Zipping Delay",
                    amount = "-35%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.TechWhite;
        }
        public override string GetModName()
        {
            return LarrysCards.ModInitials;
        }
    }
}