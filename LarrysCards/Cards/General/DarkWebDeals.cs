using LarrysCards.Patches;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.General
{
    class DarkWebDeals : CustomCard
    {

        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            statModifiers.health = 0.7f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            LarrysCards_CardChoicesPatch.AddForcedCardChoice(player, new ForcedCardRequest
            {
                customRoll = (player) =>
                {
                    return ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(
                        player,
                        player.data.weaponHandler.gun,
                        player.data.weaponHandler.gun.GetComponent<GunAmmo>(),
                        player.data,
                        player.data.healthHandler,
                        player.GetComponent<Gravity>(),
                        player.data.block,
                        player.data.GetComponent<CharacterStatModifiers>(),
                        Conditions.NotCommonOrUncommonCondition
                    );
                },
                slot = 0,
                fill = true
            });

            LarrysMod.LarrysMod.instance.PlayerDrawsIncrease(player, 1);
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            LarrysCards_CardChoicesPatch.RemoveForcedCardChoice(player, new ForcedCardRequest
            {
                customRoll = (player) =>
                {
                    return ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(
                        player,
                        player.data.weaponHandler.gun,
                        player.data.weaponHandler.gun.GetComponent<GunAmmo>(),
                        player.data,
                        player.data.healthHandler,
                        player.GetComponent<Gravity>(),
                        player.data.block,
                        player.data.GetComponent<CharacterStatModifiers>(),
                        Conditions.NotCommonOrUncommonCondition
                    );
                },
                slot = 0,
                fill = true
            });

            LarrysMod.LarrysMod.instance.PlayerDrawsIncrease(player, -1);
        }

        protected override string GetTitle()
        {
            return "Dark Web Deals";
        }
        protected override string GetDescription()
        {
            return "Your card draws will always have a <color=#ff00f4>RARE+</color> card";
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
                    stat = "Card Draws",
                    amount = "+1",
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