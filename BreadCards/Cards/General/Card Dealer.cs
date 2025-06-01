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

namespace BreadCards.Cards
{
    class CardDealer : CustomCard
    {
        public bool CommonCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            return (card.rarity == CardInfo.Rarity.Common);

        }

        public bool NotHas(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            return (!player.data.currentCards.Contains(card) && card.allowMultiple);

        }

        CardInfo[] gottencards;
        Player[] GottenCardPlayer = new Player[20];
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gottencards = new CardInfo[0];
            cardInfo.GetAdditionalData().canBeReassigned = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                Player targetplayer = PlayerManager.instance.players[i];
                if (targetplayer.teamID == player.teamID)
                {
                    CardInfo randomCard1 = ModdingUtils.Utils.Cards.instance.NORARITY_GetRandomCardWithCondition(targetplayer, gun, gunAmmo, data, health, gravity, block, characterStats, NotHas);
                    if (randomCard1 == null)
                    {
                        // if there is no valid card, then try drawing from the list of all cards (inactive + active) but still make sure it is compatible
                        CardInfo[] allCards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList().Concat((List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToArray();
                        randomCard1 = ModdingUtils.Utils.Cards.instance.DrawRandomCardWithCondition(allCards, targetplayer, null, null, null, null, null, null, null, NotHas);
                    }
                    GottenCardPlayer.AddItem(targetplayer);
                    gottencards.AddItem(randomCard1);
                    GottenCardPlayer.AddItem(targetplayer);
                    gottencards.AddItem(randomCard1);

                    player.data.maxHealth *= 1.10f;

                    ModdingUtils.Utils.Cards.instance.AddCardsToPlayer(targetplayer, new CardInfo[] { randomCard1, randomCard1 }, null, null, null, null, addToCardBar: true);
                    ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(targetplayer, randomCard1);
                }
                else
                {
                    CardInfo randomCard1 = ModdingUtils.Utils.Cards.instance.NORARITY_GetRandomCardWithCondition(targetplayer, gun, gunAmmo, data, health, gravity, block, characterStats, this.CommonCondition);
                    if (randomCard1 == null)
                    {
                        // if there is no valid card, then try drawing from the list of all cards (inactive + active) but still make sure it is compatible
                        CardInfo[] allCards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList().Concat((List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToArray();
                        randomCard1 = ModdingUtils.Utils.Cards.instance.DrawRandomCardWithCondition(allCards, targetplayer, null, null, null, null, null, null, null, this.CommonCondition);
                    }
                    GottenCardPlayer.AddItem(targetplayer);
                    gottencards.AddItem(randomCard1);
                    player.data.maxHealth *= 1.10f;
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(targetplayer, randomCard1, addToCardBar: true);
                    ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(targetplayer, randomCard1);
                }
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            for (int g = 0; g < PlayerManager.instance.players.Count; g++)
            {
                Player targetplayer = PlayerManager.instance.players[g];
                for (int i = 0; i < gottencards.Length; i++)
                {
                    if (GottenCardPlayer[i] == targetplayer)
                    {
                        ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(targetplayer, gottencards[i], ModdingUtils.Utils.Cards.SelectionType.Oldest);
                        gottencards[i] = null;
                        GottenCardPlayer[i] = null;
                    }
                }
            }
        }

        protected override string GetTitle()
        {
            return "Unfair Card Dealer";
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
                    stat = "Health / Player",
                    amount = "+10%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "",
                    amount = "",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Get",
                    amount = "You & Teammates",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
                 new CardInfoStat()
                {
                    positive = true,
                    stat = "Card & copy",
                    amount = "+1",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "",
                    amount = "",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Get",
                    amount = "Enemies",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
                 new CardInfoStat()
                {
                    positive = true,
                    stat = "Common card",
                    amount = "+1",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
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