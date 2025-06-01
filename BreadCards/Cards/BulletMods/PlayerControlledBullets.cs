using HarmonyLib;
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
using static UnityEngine.UI.GridLayoutGroup;

namespace BreadCards.Cards
{
    class PlayerControlledBullets : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            gun.damage = 0.5f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            GameObject obj = new GameObject("PlayerControlEffect", typeof(PlayerControlEffect));

            PlayerControlEffect.owner = player;

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
            return "Player Controlled Bullets";
        }
        protected override string GetDescription()
        {
            return "When you move your bullets move in the same direction";
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


    public class PlayerControlEffect : MonoBehaviour
    {

        public static Player owner;

        bool start = false;

        private MoveTransform moveTransform;
        private PhotonView photonView;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<PlayerControlEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<PlayerControlEffect>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            this.ExecuteAfterSeconds(0.2f, () =>
            {
                start = true;
            });

            
            photonView = GetComponent<PhotonView>();

            moveTransform = GetComponent<MoveTransform>();
        }

        public void Update()
        {
            if (!start) return;

            if (photonView != null)
            {

                if (owner == null) return;

                Vector2 velocity = (Vector2)Traverse.Create(owner.data.playerVel).Field("velocity").GetValue();

                if (owner.data.input.direction != Vector3.zero || owner.data.sinceJump < 0.2f)
                {

                    moveTransform.velocity += (Vector3)velocity/20 * owner.data.weaponHandler.gun.projectielSimulatonSpeed;
                }

            }
        }
    }
}