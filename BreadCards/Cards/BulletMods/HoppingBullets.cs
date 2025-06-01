using ModdingUtils.Utils;
using ModsPlus;
using Photon.Pun;
using RWF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards
{
    class HoppingBullets : CustomCard
    {
        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 1.5f;
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            GameObject obj = new GameObject("HopShotss", typeof(HopShots));

            HopShots.ownerID = player.playerID;

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
            return "Player Air Bounce Bullets";
        }
        protected override string GetDescription()
        {
            return "Your bullets will bounce off of your x & y axis";
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
                    stat = "DMG",
                    amount = "+50%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.FirepowerYellow;
        }
        public override string GetModName()
        {
            return BreadCards.ModInitials;
        }
    }

    public class HopShots : MonoBehaviour
    {
        public static int ownerID;

        public Player owner;

        private MoveTransform moveTransform;
        private PhotonView photonView;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<HopShots>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<HopShots>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            this.ExecuteAfterSeconds(0.01f, () =>
            {
                photonView = GetComponent<PhotonView>();
                moveTransform = GetComponent<MoveTransform>();
                while (owner == null) { owner = PlayerManager.instance.GetPlayerWithID(ownerID); }

                this.ExecuteAfterSeconds(0.3f, () =>
                {
                    start = true;

                    delay = new bool[] { false, false };
                });
            });
        }
        bool[] delay;
        bool start = false;
        public void Update()
        {
            if (!start) return;

            if (photonView != null)
            {

                float tolerance = 0.8f;

                if (!delay[0])
                {

                    if (transform.position.x >= owner.transform.position.x - tolerance &&
                    transform.position.x <= owner.transform.position.x + tolerance)
                    {
                        moveTransform.velocity.x *= -1f;
                        delay[0] = true;
                        Undelay(0);
                    }
                }

                if (!delay[1])
                {

                    if (transform.position.y >= owner.transform.position.y - tolerance &&
                        transform.position.y <= owner.transform.position.y + tolerance)
                    {
                        moveTransform.velocity.y *= -1f;
                        delay[1] = true;
                    }
                }
            }
        }

        void Undelay(int id)
        {
            this.ExecuteAfterSeconds(0.2f, () =>
            {
                delay[id] = false;
            });
        }
    }
}