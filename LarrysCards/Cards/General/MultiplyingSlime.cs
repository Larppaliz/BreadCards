using LarrysCards.Patches;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.General
{
    class MultiplyingSlime : CustomCard
    {
        public static CardInfo CardInfo;
        public override void Callback()
        {
            if (!LarrysCards_CardExtraInfoPatch.extraInfoCardData.ContainsKey(CardInfo.cardName))
                LarrysCards_CardExtraInfoPatch.extraInfoCardData.Add(CardInfo.cardName, _ => MultipliedSlime.CardInfo);
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 1.2f;
            statModifiers.health = 1.2f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            if (player.gameObject.GetComponent<AddCardPickStart>() == null)
            {
                player.gameObject.AddComponent<AddCardPickStart>().card = MultipliedSlime.CardInfo;
            }

            player.gameObject.GetComponent<AddCardPickStart>().chance += 35;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Destroy(player.gameObject.GetComponent<AddCardPickStart>());
        }

        protected override string GetTitle()
        {
            return "Multiplying Slime";
        }
        protected override string GetDescription()
        {
            return "Can generate a <color=#ff00f4>Multiplied Slime</color> card at the end of every round";
        }
        protected override GameObject GetCardArt()
        {
            return null;
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
                    stat = "Slime Chance",
                    amount = "+35%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Health",
                    amount = "+20%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "DMG",
                    amount = "+20%",
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