using LarrysCards.Patches;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.General
{
    class RiceMarket : CustomCard
    {
        public static CardInfo CardInfo;
        public override void Callback()
        {
            if (!LarrysCards_CardExtraInfoPatch.extraInfoCardData.ContainsKey(CardInfo.cardName))
                LarrysCards_CardExtraInfoPatch.extraInfoCardData.Add(CardInfo.cardName, _ => Rice.CardInfo);
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                Player targetplayer = PlayerManager.instance.players[i];

                LarrysCards_CardChoicesPatch.AddForcedCardChoice(targetplayer, new ForcedCardRequest
                {
                    card = Rice.CardInfo,
                    slot = 0,
                    fill = true,
                    reverse = true
                });

                if (targetplayer.teamID == player.teamID) LarrysMod.LarrysMod.instance.PlayerDrawsIncrease(player, 1);
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                Player targetplayer = PlayerManager.instance.players[i];

                LarrysCards_CardChoicesPatch.RemoveForcedCardChoice(targetplayer, new ForcedCardRequest
                {
                    card = Rice.CardInfo,
                    slot = 0,
                    fill = true,
                    reverse = true
                });

                if (targetplayer.teamID == player.teamID) LarrysMod.LarrysMod.instance.PlayerDrawsIncrease(player, -1);
            }
        }

        protected override string GetTitle()
        {
            return "Rice Market";
        }
        protected override string GetDescription()
        {
            return "Everyone always gets a <color=#5c7c9c>RICE</color> card";
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
                    stat = "Get",
                    amount = "<color=#00ff00>You & Teammates</color>",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
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