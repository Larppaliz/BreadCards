using ClassesManagerReborn.Util;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.Classes.Magnet
{
    class LessMagnetDelay : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;
        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = MagnetClass.name;
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            MagnetData mData = MagnetShot.stats[player.playerID];

            mData.magnetDelay -= 0.05f;
            mData.magnetRange += 0.5f;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            MagnetData mData = MagnetShot.stats[player.playerID];

            mData.magnetDelay += 0.05f;
            mData.magnetRange -= 0.5f;
        }
        protected override string GetTitle()
        {
            return "Quick Magnets";
        }
        protected override string GetDescription()
        {
            return "";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.MagnetArt;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Magnet Delay",
                    amount = "-0.05s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Magnet Range",
                    amount = "+0.5m",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
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