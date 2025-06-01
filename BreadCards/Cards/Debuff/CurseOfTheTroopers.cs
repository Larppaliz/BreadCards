using UnboundLib.Cards;
using UnityEngine;

using PickNCards;
using ModdingUtils;
using UnboundLib.GameModes;
using ModsPlus;

namespace BreadCards.Cards
{
    class CurseOfTheTroopers : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.spread = 0;
            data.maxHealth += 40f;
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                Player targetplayer = PlayerManager.instance.players[i];
                if (targetplayer.teamID != player.teamID)
                {
                    CardInfo givenCard = TrooperCurse.CardInfo;

                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(targetplayer, givenCard, false, givenCard.GetAbbreviation(), 0, 0);
                    ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(player, givenCard);
                }
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Plot Armor";
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
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Health",
                    amount = "+40",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                },
                                new CardInfoStat()
                {
                    positive = true,
                    stat = "Spread",
                    amount = "Reset",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
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
                    positive = false,
                    stat = "Get",
                    amount = "Enemies",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                },
                                new CardInfoStat()
                {
                    positive = false,
                    stat = "Spread",
                    amount = "60°",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
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