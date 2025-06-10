using UnboundLib.Cards;
using UnityEngine;
using System.Linq;
using ModdingUtils.Extensions;
using ModdingUtils.Utils;

namespace BreadCards.Cards.General
{
    class CardUpgrade : CustomCard
    {
        public CardInfo oldCard;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
        }

        public bool HigherRarityCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            RarityLib.Utils.Rarity rarity = RarityLib.Utils.RarityUtils.GetRarityData(card.rarity);
            CardInfo.Rarity newRarity = RarityLib.Utils.RarityUtils.GetRarity("Legendary");


            for (int i = 0; i < RarityLib.Utils.RarityUtils.Rarities.Count; i++)
            {
                if (RarityLib.Utils.RarityUtils.Rarities[i] == rarity)
                {
                    System.Random rand = new System.Random();
                    newRarity = RarityLib.Utils.RarityUtils.GetRarity(RarityLib.Utils.RarityUtils.Rarities[i + (RarityLib.Utils.RarityUtils.Rarities.Count-i-1)].name);
                }
            }

            return card.rarity == newRarity;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.damage += 15f / 55f;

            oldCard = player.data.currentCards.Last();

            CardInfo card = BlackMarketCopy.CardInfo;

            if (oldCard != null)
            {
                card = ModdingUtils.Utils.Cards.instance.NORARITY_GetRandomCardWithCondition(player, gun, gunAmmo, data, health, gravity, block, characterStats, HigherRarityCondition);

                if (card == null)
                {
                    card = ModdingUtils.Utils.Cards.instance.NORARITY_GetRandomCardWithCondition(player, gun, gunAmmo, data, health, gravity, block, characterStats, null);
                }
                    ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, player.data.currentCards.Count-1);

                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, addToCardBar: true);
                    CardBarUtils.instance.ShowAtEndOfPhase(player, card);
            }

        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Card Upgrade";
        }
        protected override string GetDescription()
        {
            return "Replace your newest card with a random one of a higher rarity";
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
                    stat = "Damage",
                    amount = "+15",
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