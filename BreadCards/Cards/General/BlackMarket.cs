using UnboundLib.Cards;
using UnityEngine;
using PickNCards;
using ModdingUtils;
using UnboundLib.GameModes;
using ModsPlus;
using BreadCards;
using System.Linq;
using SelectAnyNumberRounds;

namespace BreadCards.Cards
{
    class BlackMarket : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            BreadCards.instance.PlayerDrawsIncrease(player, 2);

            foreach (Player target in PlayerManager.instance.GetPlayersInTeam(player.teamID))
            {
                if (target != null && target != player)
                {
                    CardInfo card = BlackMarketCopy.CardInfo;
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(target, card, false, card.GetAbbreviation(), 0, 0);
                    ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(target, card);
                }
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            BreadCards.instance.PlayerDrawsIncrease(player, -2);
        }
        protected override string GetTitle()
        {
            return "Black Market";
        }
        protected override string GetDescription()
        {
            return "";
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
                    stat = "Card Draws",
                    amount = "+2",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                 new CardInfoStat()
                {
                    positive = false,
                    stat = " ",
                    amount = " ",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Get",
                    amount = "Teammates",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
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