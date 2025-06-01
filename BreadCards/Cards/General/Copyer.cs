using UnboundLib.Cards;
using UnityEngine;

using PickNCards;
using ModdingUtils;
using UnboundLib.GameModes;
using ModsPlus;
using System.Collections.ObjectModel;
using UnboundLib.Utils;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using ModdingUtils.Extensions;
using HarmonyLib;
using ModdingUtils.Utils;
using CardThemeLib;

namespace BreadCards.Cards
{
    class Copyer : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.allowMultiple = false;

            gun.damage = 0.75f;

            statModifiers.health = 0.75f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            System.Random random = new System.Random();

            CardInfo card = null;


            for (int i = player.data.currentCards.Count-1; i >= 0; i--)
            {
                card = player.data.currentCards[i];

                if (card != null)
                {
                    if (card.allowMultiple)
                    {
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, addToCardBar: true);
                        CardBarUtils.instance.ShowAtEndOfPhase(player, card);

                        return;
                    }
                }
            }

            if (card == null)
            {
                card = Rice.CardInfo;
            }


            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, addToCardBar:true);
            CardBarUtils.instance.ShowAtEndOfPhase(player,card);

        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Card Copier";
        }
        protected override string GetDescription()
        {
            return "Get a copy of your newest card that allows duplicates";
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
                    positive = false,
                    stat = "DMG",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.lower
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Health",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.lower
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