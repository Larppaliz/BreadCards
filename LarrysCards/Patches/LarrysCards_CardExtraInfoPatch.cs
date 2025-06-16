using ClassesManagerReborn.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnityEngine;

namespace LarrysCards.Patches
{

    public class CardExtraInfo : MonoBehaviour
    {
        public Func<Player, CardExtraInfo, CardInfo> propCard;

        
    }

    [HarmonyPatch(typeof(CardChoice), "Update")]
    public static class LarrysCards_CardExtraInfoPatch
    {
        public static GameObject shownObject;
        public static int selectedCard = -1;
        public static int spawnedCount = 0;

        public static List<CardInfo> getCardsFromGameObjects(List<GameObject> gameObjects)
        {
            List<CardInfo> cards = new List<CardInfo>();

            foreach (GameObject obj in gameObjects)
            {
                if (obj == null) continue;
                if (obj.GetComponent<CardInfo>())
                {
                    CardInfo info = obj.GetComponent<CardInfo>();
                    cards.Add(info);
                }
            }

            return cards;
        }

        public static void DestroyObject()
        {
            selectedCard = -1;

            if (shownObject == null) return;

            GameObject.Destroy(shownObject);

            shownObject = null;
        }

        public static void Postfix(ref CardChoice __instance, ref int ___currentlySelectedCard, ref List<GameObject> ___spawnedCards, ref Transform[] ___children)
        {
            if (CardChoiceVisuals.instance != null)
            {

                if (__instance.IsPicking)
                {

                    int index = ___currentlySelectedCard;


                    int count = ___spawnedCards.Count;

                    if (spawnedCount != count) { DestroyObject(); }

                    spawnedCount = count;

                    if (index != selectedCard)
                    {
                        DestroyObject();
                    }

                    selectedCard = index;

                    if (shownObject == null)
                    {
                        List<CardInfo> cards = getCardsFromGameObjects(___spawnedCards);

                        if (index < 0 || index > ___spawnedCards.Count-1) return;

                        GameObject spawnedCard = ___spawnedCards[index];

                        Player player = PlayerManager.instance.players[__instance.pickrID];

                        if (player == null) return;

                        if (cards.Count - 1 < index || index < 0) return;

                        if (spawnedCard == null) return;

                        if (spawnedCard.GetComponent<CardExtraInfo>() == null) return;

                        CardExtraInfo extraInfo = spawnedCard.GetComponent<CardExtraInfo>();

                        if (extraInfo.propCard != null)
                        {
                            CardInfo propCard = extraInfo.propCard.Invoke(player, extraInfo);

                            Transform cardTransform = ___spawnedCards[index].transform;

                            Vector3 baseOffset = cardTransform.right;

                            if (cardTransform.position.x > 0) baseOffset *= -1f;

                            if (cardTransform.rotation.eulerAngles.z == 0) baseOffset = cardTransform.up * -1.3f;

                            shownObject = __instance.AddCardVisual(propCard, new Vector3(0, 0, 0));
                            Vector3 rightOffset = baseOffset * 12f;
                            rightOffset.x *= cardTransform.localScale.x;
                            rightOffset.y *= cardTransform.localScale.y;
                            rightOffset.z *= cardTransform.localScale.z;

                            shownObject.transform.localScale = cardTransform.localScale * 0.8f;
                            shownObject.transform.position = cardTransform.position + rightOffset;
                            shownObject.transform.AddZPosition(-15);
                            shownObject.transform.rotation = cardTransform.rotation;
                        }
                    }
                }
                else DestroyObject();
            }
        }
    }
}
