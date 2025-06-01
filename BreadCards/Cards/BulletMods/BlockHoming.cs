using ModdingUtils.Extensions;
using ModdingUtils.MonoBehaviours;
using ModdingUtils.Utils;
using ModsPlus;
using Photon.Pun;
using Photon.Pun.Simple;
using Photon.Realtime;
using RWF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using static ModdingUtils.MonoBehaviours.CounterReversibleEffect;
using static UnityEngine.UI.GridLayoutGroup;

namespace BreadCards.Cards
{
    class BlockHoming : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;

            gun.damage = 0.5f;

            block.cdMultiplier = 1.35f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            GameObject obj = new GameObject("BlockHomingEffect", typeof(BlockHomingEffect));


            BlockHomingEffect.ownerID = player.playerID;

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
            return "Activated Homing bullets";
        }
        protected override string GetDescription()
        {
            return "Your bullets will locate and launch at the nearest target when you block";
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
                    positive = false,
                    stat = "DMG",
                    amount = "-50%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Block Cooldown",
                    amount = "+35%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Launch Delay",
                    amount = "0.35s",
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


    public class BlockHomingEffect : MonoBehaviour
    {

        public static int ownerID;

        public Player owner;

        private MoveTransform moveTransform;
        private PhotonView photonView;
        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<BlockHomingEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<BlockHomingEffect>().Where(bullet => bullet != this))
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
        bool start;
        public void Update()
        {
            if (!start) return;

            if (owner == null) { owner = PlayerManager.instance.GetPlayerWithID(ownerID); return; }

            if (photonView != null)
            {
                float x = moveTransform.velocity.x;
                if (x < 0) x *= -1f;
                float y = moveTransform.velocity.y;
                if (y < 0) y *= -1f;

                backupMag = (x + y) / 2;

                if (owner.data.block.sinceBlock == 0)
                {
                    Launch();
                }
            }
        }
        float backupMag;

        public void Launch()
        {
            Player player = PlayerManager.instance.GetClosestPlayer(transform.position, false);

            if (player != null)
            {
                float x = moveTransform.velocity.x;
                if (x < 0) x *= -1f;
                float y = moveTransform.velocity.y;
                if (y < 0) y *= -1f;

                float mag = (x + y) / 2;

                if (mag <= 0) mag = backupMag;
                if (!owner.data.currentCards.Contains(BlockReverse.CardInfo) && !owner.data.currentCards.Contains(BlockStop.CardInfo))
                {
                    moveTransform.velocity *= 0.2f;
                    moveTransform.gravity *= 0.2f;
                }
                this.ExecuteAfterSeconds(0.4f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                {
                    moveTransform.gravity = 0f;
                    Vector2 vel = BreadCards.Normalize(player.transform.position - transform.position);
                    moveTransform.velocity = new Vector3(vel.x, vel.y, 0f) * 3f * mag;
                });
            }
        }
    }
}