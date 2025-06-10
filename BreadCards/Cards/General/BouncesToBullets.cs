using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.General
{
    class BouncesToBullets: CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            int x = gun.numberOfProjectiles;
            if (gun.reflects > 0)
            {
                gun.numberOfProjectiles = gun.reflects;
            }
            else
            {
                gun.numberOfProjectiles = 1;
            }
            gun.reflects = x;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Bounces to Bullets";
        }
        protected override string GetDescription()
        {
            return "Swap your bounces and bullets amounts";
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
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }
        public override string GetModName()
        {
            return BreadCards.ModInitials;
        }
    }
}