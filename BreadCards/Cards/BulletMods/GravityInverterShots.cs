using ModdingUtils.Utils;
using ModsPlus;
using Photon.Pun;
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
    class GravityInverterShots : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            GameObject obj = new GameObject("GravityInvertShotsEffects", typeof(GravityInvertShotsEffect));

            GravityInvertShotsEffect.ownerID = player.playerID;

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
            return "Gravity Inverter Shots";
        }
        protected override string GetDescription()
        {
            return "Your bullets will flip gravity every 0.5s";
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


    public class GravityInvertShotsEffect : MonoBehaviour
    {
        public static int ownerID;

        public Player owner;

        private MoveTransform moveTransform;
        private PhotonView photonView;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<GravityInvertShotsEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<GravityInvertShotsEffect>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }

            this.ExecuteAfterSeconds(0.01f, () =>
            {
                photonView = GetComponent<PhotonView>();
                moveTransform = GetComponent<MoveTransform>();
                while (owner == null) { owner = PlayerManager.instance.GetPlayerWithID(ownerID); }

                if (photonView != null && moveTransform != null)
                {
                    Flip();
                }
            });
        }
        public void Update()
        {
        }

        public void Flip()
        {
            this.ExecuteAfterSeconds(0.5f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
            {
                Flip();
                moveTransform.gravity *= -1f;
            });
        }
    }
}