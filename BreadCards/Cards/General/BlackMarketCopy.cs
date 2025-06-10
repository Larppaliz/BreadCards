using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.General
{
    class BlackMarketCopy : CustomCard
    {
        public static CardInfo CardInfo;
        public override bool GetEnabled() => false;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            LarrysMod.LarrysMod.instance.PlayerDrawsIncrease(player, 2);

        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            LarrysMod.LarrysMod.instance.PlayerDrawsIncrease(player, 2);
        }

        protected override string GetTitle()
        {
            return "Black Market Benefits";
        }
        protected override string GetDescription()
        {
            return "I wouldnt trust your friend over there.";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.BadBlackMarketArt;
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
                    stat = "Card Draws",
                    amount = "+1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return BreadCards.ModInitials;
        }
    }
}