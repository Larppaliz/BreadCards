using ClassesManagerReborn;
using ClassesManagerReborn.Util;
using LarrysCards.Patches;
using Photon.Pun.UtilityScripts;
using System.Linq;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.General
{
    class UltimateCopyCat : CustomCard
    {

        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            statModifiers.health = 0.7f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            LarrysCards_CardChoicesPatch.AddForcedCardChoice(player, new ForcedCardRequest
            {
                customRoll = (requestingPlayer) =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        var allPlayers = PlayerManager.instance.players;

                        var candidates = allPlayers
                            .Where(p => p != requestingPlayer && p.data.currentCards.Count > 0)
                            .ToList();

                        if (candidates.Count == 0)
                            return null;

                        var rand = new System.Random();
                        var randomPlayer = candidates[rand.Next(candidates.Count)];
                        int randNumber = Random.Range(0, randomPlayer.data.currentCards.Count);
                        CardInfo card = null;

                        card = randomPlayer.data.currentCards[randNumber];

                        if (card == null) continue;

                        bool allowed = true;
                        if (ClassesRegistry.Get(card) != null)
                        {
                            allowed = ClassesRegistry.Get(card).PlayerIsAllowedCard(player);
                        }
                        if (allowed) return card;
                    }

                    return null;
                },
                slot = 0,
                fill = true
            });
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            LarrysCards_CardChoicesPatch.RemoveForcedCardChoice(player, new ForcedCardRequest
            {
                customRoll = (requestingPlayer) =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        var allPlayers = PlayerManager.instance.players;

                        var candidates = allPlayers
                            .Where(p => p != requestingPlayer && p.data.currentCards.Count > 0)
                            .ToList();

                        if (candidates.Count == 0)
                            return null;

                        var rand = new System.Random();
                        var randomPlayer = candidates[rand.Next(candidates.Count)];
                        int randNumber = Random.Range(0, randomPlayer.data.currentCards.Count);
                        CardInfo card = null;
                        card = card.GetCopyOf(randomPlayer.data.currentCards[randNumber]);

                        if (card == null) continue;

                        if (LarrysCards.allowCard(player,card))
                            return card;
                    }

                    return null;
                },
                slot = 0,
                fill = true
            });
        }

        protected override string GetTitle()
        {
            return "Ultimate Copycat";
        }
        protected override string GetDescription()
        {
            return "Your draws will have a copy of a random <color=#ff0000>Enemies</color> card.";
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
                    stat = "Health",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                }
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