using ClassesManagerReborn.Util;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.Classes.Magnet
{
    class MagnetTeleport : CustomCard
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

            mData.magnetTP = true;
            mData.magnetRange -= 1f;
            mData.magnetDelay *= 2f;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            MagnetData mData = MagnetShot.stats[player.playerID];

            mData.magnetTP = false;
            mData.magnetRange += 1f;
            mData.magnetDelay /= 2f;
        }
        protected override string GetTitle()
        {
            return "Zipping Magnets";
        }
        protected override string GetDescription()
        {
            return "Makes your magnets zip to the target position when magnetizing";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.MagnetArt;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Magnet Range",
                    amount = "-1m",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Magnet Delay",
                    amount = "+100%",
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