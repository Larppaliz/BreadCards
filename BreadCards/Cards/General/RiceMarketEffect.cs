using HarmonyLib;
using ModdingUtils.Extensions;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using SelectAnyNumberRounds.Cards;
using SelectAnyNumberRounds;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnityEngine;

namespace BreadCards.Cards
{
    class RiceMarketEffect : CustomCard
    {
        public override bool GetEnabled() => false;

        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Rice Market Effect";
        }
        protected override string GetDescription()
        {
            return "Replace last card pick option with rice on your draw phase";
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
    [HarmonyPatch(typeof(CardChoice), "SpawnUniqueCard")]
    public static class LastCardContinue
    {
        [HarmonyPriority(0)]
        [HarmonyAfter(new string[] { "com.Root.Null", "com.willuwontu.rounds.cards", "pykess.rounds.plugins.cardchoicespawnuniquecardpatch", "pykess.rounds.plugins.pickphaseshenanigans" })]
        public static void Postfix(ref CardChoice __instance, ref GameObject __result, ref List<GameObject> ___spawnedCards, ref Transform[] ___children, ref int ___pickrID)
        {
            if (PlayerManager.instance.players[___pickrID].data.currentCards.Contains(RiceMarketEffect.CardInfo))
            {
                CardInfo card = Rice.CardInfo;

                if (___spawnedCards.Count == ___children.Length - 1)
                {


                    GameObject old = __result;
                    Plugin.instance.ExecuteAfterFrames(3, delegate
                    {
                        PhotonNetwork.Destroy(old);
                    });
                    Plugin.Logger.LogDebug("Spawning rice card");
                    __result = (GameObject)typeof(CardChoice).GetMethod("Spawn", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[3]
                    {
                    card.gameObject,
                    __result.transform.position,
                    __result.transform.rotation
                    });
                    __result.GetComponent<CardInfo>().sourceCard = card;
                    __result.GetComponentInChildren<DamagableEvent>().GetComponent<Collider2D>().enabled = false;
                }
            }
        }
    }
}