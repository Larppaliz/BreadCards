using ClassesManagerReborn;
using UnityEngine;

namespace LarrysCards
{
    internal static class Conditions
    {
        public static bool CommonCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            bool allowed = true;
            if (ClassesRegistry.Get(card) != null)
            {
                allowed = ClassesRegistry.Get(card).PlayerIsAllowedCard(player);
            }
            return (card.rarity == CardInfo.Rarity.Common || card.rarity == CardInfo.Rarity.Uncommon) && allowed;

        }

        public static bool RareCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            bool allowed = true;
            if (ClassesRegistry.Get(card) != null)
            {
                allowed = ClassesRegistry.Get(card).PlayerIsAllowedCard(player);
            }
            return card.rarity == CardInfo.Rarity.Rare && allowed;
        }

        public static bool NotCommonOrUncommonCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            bool allowed = true;
            if (ClassesRegistry.Get(card) != null)
            {
                allowed = ClassesRegistry.Get(card).PlayerIsAllowedCard(player);
            }
            return (card.rarity != CardInfo.Rarity.Common  && card.rarity != CardInfo.Rarity.Uncommon) && allowed;
        }

        public static bool AnyCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            bool allowed = true;
            if (ClassesRegistry.Get(card) != null)
            {
                allowed = ClassesRegistry.Get(card).PlayerIsAllowedCard(player);
            }
            return (!LarrysCards.MyCursedCards.Contains(card)) && allowed;
        }
    }
}
