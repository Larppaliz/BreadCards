using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.General
{
    class BlackMarket : CustomCard
    {
        public static CardInfo CardInfo;
        public override void Callback()
        {
            if (!BreadCards_CardExtraInfoPatch.extraInfoCardData.ContainsKey(CardInfo.cardName))
                BreadCards_CardExtraInfoPatch.extraInfoCardData.Add(CardInfo.cardName, _ => BlackMarketCopy.CardInfo);
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            LarrysMod.LarrysMod.instance.PlayerDrawsIncrease(player, 2);

            foreach (Player target in PlayerManager.instance.GetPlayersInTeam(player.teamID))
            {
                if (target != null && target != player)
                {
                    CardInfo card = BlackMarketCopy.CardInfo;
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(target, card, false, "", 0, 0);
                    ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(target, card);
                }
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            LarrysMod.LarrysMod.instance.PlayerDrawsIncrease(player, -2);
        }
        protected override string GetTitle()
        {
            return "Black Market";
        }
        protected override string GetDescription()
        {
            return "<color=#00ff00>Teammates</color> get a <color=#5c7c9c>Black Market Benefits</color> card";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.BlackMarketArt;
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
                    amount = "+2",
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