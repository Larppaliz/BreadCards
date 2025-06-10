using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.General
{
    class BoosterPack : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            player.gameObject.AddComponent<BoosterPackEffect>().card = cardInfo;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            BoosterPackEffect effect = player.gameObject.GetComponent<BoosterPackEffect>();
            Destroy(effect);
        }

        protected override string GetTitle()
        {
            return "Booster Pack";
        }
        protected override string GetDescription()
        {
            return "Win the next round to open";
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
                    stat = "Cards",
                    amount = "2",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Rare Card",
                    amount = "1",
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
            return BreadCards.ModInitials;
        }
    }
    public class BoosterPackEffect : MonoBehaviour
    {
        public Player player;
        public CardInfo card;
        public void Start()
        {
            player = gameObject.GetComponentInParent<Player>();
        }
    }
}