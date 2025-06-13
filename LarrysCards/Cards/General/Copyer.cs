using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.Extensions;
using ModdingUtils.Utils;
using LarrysCards.Patches;

namespace LarrysCards.Cards.General
{
    class Copier : CustomCard
    {
        public static CardInfo CardInfo;
        public override void Callback()
        {
            if (!LarrysCards_CardExtraInfoPatch.extraInfoCardData.ContainsKey(CardInfo.cardName))
                LarrysCards_CardExtraInfoPatch.extraInfoCardData.Add(CardInfo.cardName, (player) =>
                {
                    CardInfo card = null;


                    for (int i = player.data.currentCards.Count - 1; i >= 0; i--)
                    {
                        card = player.data.currentCards[i];

                        if (card != null)
                        {
                            if (LarrysCards.allowCard(player, card))
                            {
                                break;
                            }
                        }
                    }

                    if (card == null)
                    {
                        card = Rice.CardInfo;
                    }

                    return card;
                });
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.allowMultiple = false;
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
                        break;
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
            return Assets.CardCopierArt;
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