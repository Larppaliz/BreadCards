using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.Debuff
{
    class EvilCurse : CustomCard
    {
        public static CardInfo CardInfo { get; internal set; }

        public override bool GetEnabled() => false;

        

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            enabled = false;
            block.cdAdd = 0.5f;
            gun.damage = 0.8f;
            cardInfo.enabled = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.unblockable = false;
            if (data.maxHealth - 40f > 0)
            {
                data.maxHealth -= 40f;
            }
            else
            {
                data.maxHealth = 1f;
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Evil Curse";
        }
        protected override string GetDescription()
        {
            return "";
        }
        protected override GameObject GetCardArt()
        {
            return null;
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
                    positive = false,
                    stat = "DMG",
                    amount = "-20%",
                    simepleAmount = CardInfoStat.SimpleAmount.Some
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Health",
                    amount = "-35",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Block Cooldown",
                    amount = "+0.5s",
                    simepleAmount = CardInfoStat.SimpleAmount.Some
                },
                                new CardInfoStat()
                {
                    positive = false,
                    stat = "Bullets",
                    amount = "Blockable",
                    simepleAmount = CardInfoStat.SimpleAmount.Some
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return BreadCards.ModInitials;
        }
    }
}