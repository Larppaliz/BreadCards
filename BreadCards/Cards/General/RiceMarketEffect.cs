using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.General
{
    class RiceMarketEffect : CustomCard
    {
        public override bool GetEnabled() => false;

        public static CardInfo CardInfo;

        public override void Callback()
        {
            if (!BreadCards_CardExtraInfoPatch.extraInfoCardData.ContainsKey(CardInfo.cardName))
                BreadCards_CardExtraInfoPatch.extraInfoCardData.Add(CardInfo.cardName, _ => Rice.CardInfo);
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            BreadCards_CardChoicesPatch.AddForcedCardChoice(player, new ForcedCardRequest
            {
                card = Rice.CardInfo,
                slot = 0,
                fill = true,
                reverse = true
            });
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            BreadCards_CardChoicesPatch.RemoveForcedCardChoice(player, new ForcedCardRequest
            {
                card = Rice.CardInfo,
                slot = 0,
                fill = true,
                reverse = true
            });
        }

        protected override string GetTitle()
        {
            return "Riced Up";
        }
        protected override string GetDescription()
        {
            return "Replace one of your card options with a <color=#5c7c9c>RICE</color> card";
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
}