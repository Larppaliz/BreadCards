using ModdingUtils.Utils;
using ModsPlus;
using Photon.Pun;
using Photon.Pun.Simple;
using RWF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace BreadCards.Cards
{
    class BlockStop : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;

            block.cdMultiplier = 0.65f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            GameObject obj = new GameObject("BlockStopEffect", typeof(BlockStopEffect));

            BlockStopEffect.ownerID = player.playerID;

            List<ObjectsToSpawn> list = gun.objectsToSpawn.ToList();
            list.Add(new ObjectsToSpawn
            {
                AddToProjectile = obj
            });
            gun.objectsToSpawn = list.ToArray();
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

        }
        protected override string GetTitle()
        {
            return "Activated Static bullets";
        }
        protected override string GetDescription()
        {
            return "Your bullets will stop in place when you block";
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
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Block Cooldown",
                    amount = "-35%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
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


    public class BlockStopEffect : MonoBehaviour
    {

        public static int ownerID;

        public Player owner;

        bool start;

        private MoveTransform moveTransform;
        private PhotonView photonView;
        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<BlockStopEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<BlockStopEffect>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }


            this.ExecuteAfterSeconds(0.01f, () =>
            {
                start = true;
                photonView = GetComponent<PhotonView>();
                moveTransform = GetComponent<MoveTransform>();
                owner = PlayerManager.instance.GetPlayerWithID(ownerID);
            });
        }
        public void Update()
        {
            if (!start) return;

            if (owner == null) { owner = PlayerManager.instance.GetPlayerWithID(ownerID); return; }

            if (photonView != null)
            {
                if (owner != null)
                {
                    if (owner.data.block.sinceBlock == 0)
                    {
                        moveTransform.velocity *= 0.0001f;
                    }
                }
            }
        }
    }
}