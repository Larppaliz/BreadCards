using LarrysCards.Patches;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.General
{
    class DoubleIt : CustomCard
    {
        public static CardInfo CardInfo;
        public override void Callback()
        {
            if (!LarrysCards_CardExtraInfoPatch.extraInfoCardData.ContainsKey(CardInfo.cardName))
                LarrysCards_CardExtraInfoPatch.extraInfoCardData.Add(CardInfo.cardName, _ => GiveItToMeLater.CardInfo);
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            LarrysCards_CardChoicesPatch.AddForcedCardChoice(player, new ForcedCardRequest
            {
                card = GiveItToMeLater.CardInfo,
                slot = 0,
                fill = true,
                reverse = true,
                condition = (player) =>
                {
                    return !player.gameObject.GetComponent<DualPickEffect>();
                }
            });
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            LarrysCards_CardChoicesPatch.AddForcedCardChoice(player, new ForcedCardRequest
            {
                card = GiveItToMeLater.CardInfo,
                slot = 0,
                fill = true,
                reverse = true,
                condition = (player) =>
                {
                    return !player.gameObject.GetComponent<DualPickEffect>();
                }
            });
        }

        protected override string GetTitle()
        {
            return "Double it & Give it to me later";
        }
        protected override string GetDescription()
        {
            return "Adds a <color=#62f2f7>GIVE IT TO ME LATER</color> card to your draws";
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