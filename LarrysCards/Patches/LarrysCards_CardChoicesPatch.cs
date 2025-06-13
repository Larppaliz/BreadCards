using HarmonyLib;
using LarrysCards.Cards.General;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnboundLib;
using UnityEngine;

namespace LarrysCards.Patches
{
    public class ForcedCardRequest
    {
        public ForcedCardRequest Clone()
        {
            return new ForcedCardRequest
            {
                card = this.card, // deep clone if needed
                slot = this.slot,
                fill = this.fill,
                reverse = this.reverse,
                customRoll = this.customRoll,
                condition = this.condition
            };
        }

        // condition for the card to be added to the draw that is checked each draw (see DoubleIt.cs)
        public Func<Player, bool> condition = (_) => true;

        // for special things like rolling a random card eachtime (see DarkWebDeals.cs or UltimateCopyCat.cs)
        public Func<Player, CardInfo> customRoll;

        // use if set card (see RiceMarketEffect.cs)
        public CardInfo card;

        // the slot the card is replacing
        public int slot;

        // ex: whetever or not the card will move from slot 1 to 2 if slot 1 was already overrided
        public bool fill;

        // reverses the slots so 0 is now the last card and 1 is the second last
        public bool reverse;
    }

    [HarmonyPatch(typeof(CardChoice), "SpawnUniqueCard")]
    public static class LarrysCards_CardChoicesPatch
    {
        public static Dictionary<int, List<ForcedCardRequest>> pendingForcedCards = new Dictionary<int, List<ForcedCardRequest>>();

        public static Dictionary<int, List<ForcedCardRequest>> readyForcedCards = new Dictionary<int, List<ForcedCardRequest>>();

        public static void AddForcedCardChoice(Player player, ForcedCardRequest fcr)
        {
            if (!pendingForcedCards.ContainsKey(player.playerID))
                pendingForcedCards.Add(player.playerID, new List<ForcedCardRequest>());

            pendingForcedCards[player.playerID].Add(fcr);
        }

        public static bool RemoveForcedCardChoice(Player player, ForcedCardRequest fcr)
        {
            if (!pendingForcedCards.ContainsKey(player.playerID)) return false;

            return pendingForcedCards[player.playerID].Remove(fcr);
        }
        public static void RemoveFromReadyForcedCards(int playerID, ForcedCardRequest fcr)
        {
            if (!readyForcedCards.ContainsKey(playerID))
                return;

            if (!readyForcedCards[playerID].Contains(fcr))
                return;

            readyForcedCards[playerID].Remove(fcr);
        }

        public static void ClearForcedChoices(Player player)
        {
            int playerId = PlayerManager.instance.players.IndexOf(player);

            if (pendingForcedCards == null) pendingForcedCards = new Dictionary<int, List<ForcedCardRequest>>();

            if (pendingForcedCards.ContainsKey(playerId))
            {
                pendingForcedCards.Remove(playerId);
            }
        }

        public static void ClearOverridedSlots(Player player)
        {
            int playerId = PlayerManager.instance.players.IndexOf(player);

            if (overridedSlotsThisRun == null) overridedSlotsThisRun = new Dictionary<int, List<int>>();

            if (overridedSlotsThisRun.ContainsKey(playerId))
            {
                overridedSlotsThisRun[playerId] = new List<int>();
            }
        }

        static Dictionary<int, List<int>> overridedSlotsThisRun = new Dictionary<int, List<int>>();

        public static void SetBetterSlots(Player player, int maxCount)
        {
            int[] slotCounts = new int[maxCount];

            foreach (ForcedCardRequest card in readyForcedCards[player.playerID])
            {
                if (card.reverse) card.slot = maxCount - card.slot - 1;

                if (card.slot == -1)
                {
                    int tempSlot = 0;

                    for (int i = 0; i < 100; i++)
                    {
                        if (slotCounts[tempSlot] > 0)
                        {
                            tempSlot = UnityEngine.Random.Range(0, maxCount);
                        }
                    }

                    if (slotCounts[tempSlot] == 0)
                    {
                        card.slot = tempSlot;
                    }
                    else return;

                }
                else for (int i = 0; i < maxCount; i++)
                    {
                        if (slotCounts[card.slot] > 0)
                        {
                            if (card.reverse) card.slot--;
                            else card.slot++;
                        }
                        else break;
                    }

                if (card.slot >= 0 && card.slot < maxCount) slotCounts[card.slot]++;
            }

        }



        [HarmonyPriority(0)]
        [HarmonyAfter(new string[] { "com.Root.Null", "com.willuwontu.rounds.cards", "pykess.rounds.plugins.cardchoicespawnuniquecardpatch", "pykess.rounds.plugins.pickphaseshenanigans" })]
        public static void Postfix(ref CardChoice __instance, ref GameObject __result, ref List<GameObject> ___spawnedCards, ref Transform[] ___children, ref int ___pickrID)
        {

            if (___pickrID < 0 || ___pickrID > PlayerManager.instance.players.Count || PlayerManager.instance == null || PlayerManager.instance.players == null) return;

            Player player = PlayerManager.instance.players[___pickrID];

            if (player == null || pendingForcedCards == null || __instance == null || __result == null || ___spawnedCards == null || ___children == null) return;

            if (!pendingForcedCards.ContainsKey(___pickrID)) return;

            if (overridedSlotsThisRun == null) overridedSlotsThisRun = new Dictionary<int, List<int>>();

            if (___spawnedCards.Count == 0)
            {
                readyForcedCards = pendingForcedCards.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value
                        .Where(fcr => fcr.condition(player))
                        .Select(fcr => fcr.Clone())
                        .ToList());

                SetBetterSlots(player, ___children.Count());
            }

            for (int i = 0; i < readyForcedCards[___pickrID].Count; i++)
            {
                ForcedCardRequest fcr = readyForcedCards[___pickrID][i];

                int slot = fcr.slot;


                if (___spawnedCards.Count == slot)
                {
                    CardInfo card = null;

                    if (fcr.customRoll == null) card = fcr.card;

                    if (card == null && fcr.customRoll != null)
                    {
                        card = fcr.customRoll?.Invoke(player);
                    }

                    if (card == null) { card = Rice.CardInfo; LarrysCards.print("Card was null, turned it into RICE"); }

                    if (!overridedSlotsThisRun.ContainsKey(player.playerID)) overridedSlotsThisRun.Add(player.playerID, new List<int>());



                    overridedSlotsThisRun[player.playerID].AddItem(slot);

                    RemoveFromReadyForcedCards(player.playerID, fcr);


                    GameObject old = __result;
                    if (LarrysCards.instance == null || card.gameObject == null) return;

                    LarrysCards.instance.ExecuteAfterFrames(3, delegate
                    {
                        PhotonNetwork.Destroy(old);
                    });
                    __result = (GameObject)typeof(CardChoice).GetMethod("Spawn", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[3]
                    {
                                card.gameObject,
                                __result.transform.position,
                                __result.transform.rotation
                    });
                    __result.GetComponent<CardInfo>().sourceCard = card;
                    __result.GetComponentInChildren<DamagableEvent>().GetComponent<Collider2D>().enabled = false;

                    break;
                }
            }
        }
    }

}
