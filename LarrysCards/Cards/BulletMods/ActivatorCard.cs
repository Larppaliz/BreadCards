using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.UI.GridLayoutGroup;

namespace LarrysCards.Cards.BulletMods
{
    class ActivatorCard : CustomCard
    {
        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            CardInfo = cardInfo;
            cardInfo.allowMultiple = false;

            block.cdMultiplier = 0.75f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            player.gameObject.GetOrAddComponent<ActivatorMono>().actionsEnabled = true;

            Type type = typeof(ActivatedShots);

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            GameObject obj = new GameObject("ActivatedShotsEffect", type);

            gun.objectsToSpawn = gun.objectsToSpawn.Append(new ObjectsToSpawn
            {
                AddToProjectile = obj
            }).ToArray();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            if (player.GetComponent<ActivatorMono>())
            {
                GameObject.Destroy(player.GetComponent<ActivatorMono>());
            }
        }

        protected override string GetTitle()
        {
            return "Activator";
        }
        protected override string GetDescription()
        {
            return "All <color=#9aa355>TIMED</color> cards are now instead activated on block";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.ActivatorArt;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Block Cooldown",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                }
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
    public class ActivatorMono : MonoBehaviour
    {
        public bool actionsEnabled = false;

        public List<Action<ActivatedShots, int>> actions = new List<Action<ActivatedShots, int>>();

        public static void Add(Player player, Action<ActivatedShots, int> action)
        {
            List<Action<ActivatedShots, int>> actions = player.gameObject.GetOrAddComponent<ActivatorMono>().actions;

            actions.Add(action);

            player.gameObject.GetOrAddComponent<ActivatorMono>().actions = actions;
        }

        public static void Insert(Player player, int index, Action<ActivatedShots, int> action)
        {
            List<Action<ActivatedShots, int>> actions = player.gameObject.GetOrAddComponent<ActivatorMono>().actions;

            actions.Insert(index,action);

            player.gameObject.GetOrAddComponent<ActivatorMono>().actions = actions;
        }

        public static void Prepend(Player player, Action<ActivatedShots, int> action)
        {
            List<Action<ActivatedShots, int>> actions = player.gameObject.GetOrAddComponent<ActivatorMono>().actions;

            actions.Prepend(action);

            player.gameObject.GetOrAddComponent<ActivatorMono>().actions = actions;
        }
    }
    public class ActivatedShots : MonoBehaviour
    {
        public int ownerID = -1;

        public bool isFinalAction;

        public Player owner;

        MoveTransform moveTransform;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<ActivatedShots>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<ActivatedShots>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }

            if (GetComponent<SpawnedAttack>() == null) return;

            owner = GetComponent<SpawnedAttack>().spawner;

            if (owner == null) { this.ExecuteAfterFrames(1, () => { Awake(); }); return; }

            ownerID = owner.playerID;

            moveTransform = GetComponent<MoveTransform>();

        }


        public void Update()
        {
            if (owner == null) return;

            if (owner.data.block.sinceBlock == 0)
            {
                Activate();
            }
        }
        public void Activate(int startIndex = 0)
        {
            if (moveTransform == null || owner == null) return;

            var activator = owner.gameObject.GetComponent<ActivatorMono>();
            if (activator == null || activator.actions == null) return;

            // Create a new list starting from startIndex
            List<Action<ActivatedShots, int>> actionsToExecute = activator.actions
                .Skip(startIndex)
                .ToList();

            isFinalAction = false;

            for (int i = 0; i < actionsToExecute.Count; i++)
            {
                float newtime = 0.25f * (i + 1);
                int currentIndex = i + startIndex; // Store the actual index
                var action = actionsToExecute[i];

                this.ExecuteAfterSeconds(newtime / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                {
                    if (currentIndex == activator.actions.Count - 1)
                        isFinalAction = true;

                    LarrysCards.print($"action {currentIndex+1}/{activator.actions.Count}");
                    try
                    {
                        action?.Invoke(this, currentIndex);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error executing action {currentIndex+1}: {e}");
                    }
                });
            }
        }
    }
}