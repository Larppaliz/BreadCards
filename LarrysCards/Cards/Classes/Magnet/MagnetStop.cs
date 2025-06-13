using ClassesManagerReborn.Util;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.Classes.Magnet
{
    class MagnetStop : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;
        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = "Zip "+MagnetClass.name;
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 0.75f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            MagnetData mData = MagnetShot.stats[player.playerID];

            mData.magnetStop = true;
            mData.magnetCD *= 0.65f;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            MagnetData mData = MagnetShot.stats[player.playerID];

            mData.magnetStop = false;
            mData.magnetCD /= 0.65f;
        }
        protected override string GetTitle()
        {
            return "Stopping Zip Magnets";
        }
        protected override string GetDescription()
        {
            return "Makes your magnets stop in place after zipping";
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
                    positive = true,
                    stat = "Magnet CD",
                    amount = "-35%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                                new CardInfoStat()
                {
                    positive = false,
                    stat = "DMG",
                    amount = "-25%",
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
            return LarrysCards.ModInitials;
        }
    }
}