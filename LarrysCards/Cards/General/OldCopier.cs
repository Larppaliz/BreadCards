using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.Extensions;
using ModdingUtils.Utils;
using LarrysCards.Patches;
using UnboundLib;


namespace LarrysCards.Cards.General
{
    class OldCopier : CustomCard
    {
        public static CardInfo CardInfo;
        public override void Callback()
        {
            gameObject.GetOrAddComponent<CardExtraInfo>().propCard = (player, extraInfo) =>
                {
                    CardInfo addedCard = null;

                    for (int i = 0; i < player.data.currentCards.Count; i++)
                    {
                        CardInfo card = player.data.currentCards[i];

                        if (card != null)
                        {
                            if (LarrysCards.allowCard(player, card))
                            {
                                addedCard = card;
                                break;
                            }
                        }
                    }

                    if (addedCard == null)
                    {
                        addedCard = Rice.CardInfo;
                    }

                    return addedCard;
                };
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            CardInfo addedCard = null;


            for (int i = 0; i < player.data.currentCards.Count; i--)
            {
                CardInfo card = player.data.currentCards[i];

                if (card != null)
                {
                    if (LarrysCards.allowCard(player, card))
                    {
                        addedCard = card;
                        break;
                    }
                }
            }

            if (addedCard == null)
            {
                addedCard = Rice.CardInfo;
            }

            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, addedCard, addToCardBar: true);


        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Dusty Card Copier";
        }
        protected override string GetDescription()
        {
            return "Get a copy of your oldest card that allows duplicates";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.OldCardCopierArt;
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