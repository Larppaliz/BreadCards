using Photon.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnboundLib.Utils;
using UnityEngine;

namespace BreadCards.Cards
{
    class DoubleIt : CustomCard
    {
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
            BoosterPackEffect effect = player.gameObject.GetComponent<BoosterPackEffect>();
            Destroy(effect);
        }

        protected override string GetTitle()
        {
            return "Double it and give it to me later";
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
            return BreadCards.ModInitials;
        }
    }
    public class DualPickEffect : MonoBehaviour
    {
        public Player player;
        public CardInfo card;
        public void Start()
        {
            GameModeManager.AddOnceHook(GameModeHooks.HookPickStart, BreadCards.instance.PrepareDoubleCardPick);
           
            player = gameObject.GetComponentInParent<Player>();
        }

    }


}