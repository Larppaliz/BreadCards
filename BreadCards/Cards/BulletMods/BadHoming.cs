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
    class BadHoming : CustomCard
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

            GameObject obj = new GameObject("BadHomingEffect", typeof(BadHomingEffect));

            obj.GetComponent<BadHomingEffect>();

            BadHomingEffect.ownerID = player.playerID;

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
            return "Homer bullet";
        }
        protected override string GetDescription()
        {
            return "Your bullets will attempt to home into enemies";
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


    public class BadHomingEffect : MonoBehaviour
    {
        public static int ownerID;

        public Player owner;

        bool start = false;
        bool ownerDelay = true;

        private MoveTransform moveTransform;
        private PhotonView photonView;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<BadHomingEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<BadHomingEffect>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            owner = PlayerManager.instance.GetPlayerWithID(ownerID);
            this.ExecuteAfterSeconds(0.4f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
            {
                start = true;
                this.ExecuteAfterSeconds(0.5f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                {
                    ownerDelay = false;
                });
            });

            photonView = GetComponent<PhotonView>();
            moveTransform = GetComponent<MoveTransform>();
        }
        bool ownerdelay = true;
        public void Update()
        {
            if (!start) return;

            if (owner == null) { owner = PlayerManager.instance.GetPlayerWithID(ownerID); return; }

            if (photonView != null)
            {
                Player player = PlayerManager.instance.GetClosestPlayer(transform.position, true);

                if (player != null)
                {
                    if (player == owner && ownerdelay) return;

                    if (BreadCards.Distance(transform.position, player.transform.position) < 17.5f)
                    {
                        Vector2 vel = BreadCards.Normalize(player.transform.position - transform.position);
                        moveTransform.velocity += new Vector3(vel.x, vel.y, 0f) * owner.data.weaponHandler.gun.projectielSimulatonSpeed;
                    }
                }
            }
        }
    }
}