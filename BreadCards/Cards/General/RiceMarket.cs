using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.General
{
    class RiceMarket : CustomCard
    {
        public static CardInfo CardInfo;
        public override void Callback()
        {
            if (!BreadCards_CardExtraInfoPatch.extraInfoCardData.ContainsKey(CardInfo.cardName))
                BreadCards_CardExtraInfoPatch.extraInfoCardData.Add(CardInfo.cardName, _ => Rice.CardInfo);
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                Player targetplayer = PlayerManager.instance.players[i];

                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(targetplayer, RiceMarketEffect.CardInfo, false, "Ri", 0, 0);
                ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(player, RiceMarketEffect.CardInfo);
            }

            LarrysMod.LarrysMod.instance.PlayerDrawsIncrease(player, 1);
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            LarrysMod.LarrysMod.instance.PlayerDrawsIncrease(player, -1);
        }

        protected override string GetTitle()
        {
            return "Rice Market";
        }
        protected override string GetDescription()
        {
            return "Everyone always gets a <color=#5c7c9c>RICE</color> card";
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
                    stat = "Get",
                    amount = "<color=#00ff00>You & Teammates</color>",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
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
            return CardThemeColor.CardThemeColorType.TechWhite;
        }
        public override string GetModName()
        {
            return BreadCards.ModInitials;
        }
    }
}