using UnboundLib.Cards;
using UnityEngine;
using System.Collections.Generic;
using ModdingUtils.Extensions;
using ModdingUtils.Utils;
using ClassesManagerReborn;

namespace LarrysCards.Cards.General
{
    class CardTricks : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            System.Random random = new System.Random();

            List<Player> candatites = new List<Player>();

            foreach (Player candatite in PlayerManager.instance.players)
            {
                if (candatite.data.currentCards.Count > 0)
                {
                    candatites.Add(candatite);
                }
            }


            CardInfo card = Rice.CardInfo;


            if (candatites.Count > 0)
            {

                Player target = candatites[random.Next(candatites.Count)];


                for (int i = target.data.currentCards.Count - 1; i >= 0; i--)
                {
                    card = target.data.currentCards[i];


                    if (card != null)
                    {
                        if (LarrysCards.allowCard(player, card))
                        {
                            break;
                        }
                    }
                }
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
            return "Get a copy of a random players newest card that allows duplicates.";
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