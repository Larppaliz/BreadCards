using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnityEngine;

namespace LarrysCards.Cards.General
{
    class GiveItToMeLater : CustomCard
    {
        public static CardInfo CardInfo;
        public override bool GetEnabled() => false;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            player.gameObject.AddComponent<DualPickEffect>();
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Give it to me later";
        }
        protected override string GetDescription()
        {
            return "On end of the next pick phase you get a clone of your newest card (usually the one you picked)";
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
    public class DualPickEffect : MonoBehaviour
    {
        public Player player;
        public CardInfo card;
        public void Start()
        {
            GameModeManager.AddOnceHook(GameModeHooks.HookPickStart, LarrysCards.instance.PrepareDoubleCardPick);
           
            player = gameObject.GetComponentInParent<Player>();
        }

    }


}