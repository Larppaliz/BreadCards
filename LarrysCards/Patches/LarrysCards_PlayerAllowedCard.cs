using ClassesManagerReborn;
using HarmonyLib;
using LarrysCards.Cards.BulletMods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LarrysCards.Patches
{
    [Serializable]
    [HarmonyPatch(typeof(ModdingUtils.Utils.Cards), "PlayerIsAllowedCard")]
    public class LarrysCards_PlayerAllowedCard
    {
        public static void Postfix(ref bool __result, Player player, CardInfo card)
        {
            if (!__result) return;
            if (player == null || card == null) return;
            if (card == ActivatorCard.CardInfo)
            {
                __result = player.GetComponent<ActivatorMono>();
            }
        }
    }
} 
