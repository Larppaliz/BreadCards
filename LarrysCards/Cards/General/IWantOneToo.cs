using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.General
{
    class IWantOneToo : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
               player.gameObject.AddComponent<WinnerCardEffect>();
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
                WinnerCardEffect effect = player.gameObject.GetComponent<WinnerCardEffect>();
                Destroy(effect);
        }

        protected override string GetTitle()
        {
            return "I Want One Too";
        }
        protected override string GetDescription()
        {
            return "You get a random <color=#a4a49c>COMMON</color> or <color=#62f2f7>UNCOMMON</color> card when you win";
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
            return CardThemeColor.CardThemeColorType.TechWhite;
        }
        public override string GetModName()
        {
            return LarrysCards.ModInitials;
        }
    }
    public class WinnerCardEffect : MonoBehaviour
    {
        public Player player;
        public void Start()
        {
            player = gameObject.GetComponentInParent<Player>();
        }
    }
}