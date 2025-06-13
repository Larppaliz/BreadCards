using LarrysCards.Patches;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.Debuff
{
    class StickySabotage : CustomCard
    {
        public static CardInfo CardInfo;
        public override void Callback()
        {
            if (!LarrysCards_CardExtraInfoPatch.extraInfoCardData.ContainsKey(CardInfo.cardName))
                LarrysCards_CardExtraInfoPatch.extraInfoCardData.Add(CardInfo.cardName, _ => StickySolution.CardInfo);
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 1.50f;
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                Player targetplayer = PlayerManager.instance.players[i];
                if (targetplayer.teamID != player.teamID)
                {
                    CardInfo givenCard = StickySolution.CardInfo;

                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(targetplayer, givenCard, false, "", 0, 0);
                    ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(player, givenCard);
                }
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Sticky Sabotage";
        }
        protected override string GetDescription()
        {
            return "All <color=#ff0000>Enemies</color> get a <color=#e362f7>Sticky Solution</color> card";
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
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return LarrysCards.ModInitials;
        }
    }
}