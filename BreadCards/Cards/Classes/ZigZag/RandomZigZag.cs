using ClassesManagerReborn.Util;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.Classes.ZigZag
{
    class RandomZigZag : CustomCard
    {
        public static CardInfo CardInfo;

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = ZigZagsClass.name;
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 1.5f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            ZigZagData zData = ZigZagShots.stats[player.playerID];

            zData.startDelay += 0.3f;
            zData.randomness = true;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            ZigZagData zData = ZigZagShots.stats[player.playerID];

            zData.startDelay -= 0.3f;
            zData.randomness = false;
        }

        protected override string GetTitle()
        {
            return "Random Zig Zag";
        }
        protected override string GetDescription()
        {
            return "Your bullets will go into a random direction in the spread when zig zagging instead of always the maximum";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.ZigZagArt;
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
                    stat = "Start Delay",
                    amount = "+0.3s",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "DMG",
                    amount = "+50%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.FirepowerYellow;
        }
        public override string GetModName()
        {
            return BreadCards.ModInitials;
        }
    }
}