using LarrysCards.Patches;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.General
{
    class UltraDefense : CustomCard
    {
        public static CardInfo CardInfo;
        public override void Callback()
        {
            if (!LarrysCards_CardExtraInfoPatch.extraInfoCardData.ContainsKey(CardInfo.cardName))
                LarrysCards_CardExtraInfoPatch.extraInfoCardData.Add(CardInfo.cardName, _ => UltraDefenseCopy.CardInfo);
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            statModifiers.health = 1.4f;
            block.cdAdd = -0.75f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            foreach (Player target in PlayerManager.instance.GetPlayersInTeam(player.teamID))
            {
                if (target != null && target.playerID != player.playerID)
                {
                    CardInfo card = UltraDefenseCopy.CardInfo;
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(target, card, false, "", 0, 0);
                    ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(target, card);
                }
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Ultra Defense";
        }
        protected override string GetDescription()
        {
            return "<color=#00ff00>Teammates</color> get a <color=#5c7c9c>Ultra Defended</color> card";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.UltraDefenseArt;
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
                    stat = "Health",
                    amount = "+40%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Block Cooldown",
                    amount = "-0.75s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
        }
        public override string GetModName()
        {
            return LarrysCards.ModInitials;
        }
    }
}