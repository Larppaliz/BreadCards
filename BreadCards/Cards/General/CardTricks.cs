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
    class CardTricks : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            System.Random random = new System.Random();

            Player target = PlayerManager.instance.players[random.Next(PlayerManager.instance.players.Count)];

            CardInfo card = target.data.currentCards.Last();

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
            return "Card Tricks";
        }
        protected override string GetDescription()
        {
            return "Get a copy of a random players newest card";
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
            return BreadCards.ModInitials;
        }
    }
}