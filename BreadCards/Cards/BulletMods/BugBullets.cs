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
    class BugBullets : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;

            gun.damage = 1.25f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            GameObject obj = new GameObject("BugBulletEffect", typeof(BugBulletsEffect));

            BugBulletsEffect.ownerID = player.playerID;

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
            return "Buggy Bullets";
        }
        protected override string GetDescription()
        {
            return "Your bullets will sometimes Bug around";
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
                    amount = "+25%",
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


    public class BugBulletsEffect : MonoBehaviour
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
                transform.parent.gameObject.AddComponent<BugBulletsEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<BugBulletsEffect>().Where(bullet => bullet != this))
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

                if (photonView != null && moveTransform != null)
                {
                    this.ExecuteAfterSeconds(0.2f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                    {
                        Bug();
                    });
                }
            });
        }
        public void Update()
        {
        }
        public void Bug()
        {
            this.ExecuteAfterSeconds(UnityEngine.Random.Range(0.1f, 1.2f) / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
            {
                moveTransform.velocity = BreadCards.RotatedBy(moveTransform.velocity, UnityEngine.Random.Range(-20f,20f));
                moveTransform.velocity *= UnityEngine.Random.Range(0.9f, 1.1f);

                transform.AddXPosition(UnityEngine.Random.Range(-3f, 3f));
                transform.AddYPosition(UnityEngine.Random.Range(-3f, 3f));

                moveTransform.gravity *= UnityEngine.Random.Range(0.8f, 1.2f);

                if (UnityEngine.Random.Range(0, 1f) < 0.02f)
                {
                    moveTransform.gravity *= -1f;
                }

                if (UnityEngine.Random.Range(0, 1f) < 0.03f)
                {
                    moveTransform.velocity = BreadCards.RotatedBy(moveTransform.velocity, UnityEngine.Random.Range(0f, 360f));
                }

                if (UnityEngine.Random.Range(0, 1f) < 0.01f)
                {
                    transform.AddXPosition(UnityEngine.Random.Range(-15f, 15f));
                    transform.AddYPosition(UnityEngine.Random.Range(-15f, 15f));
                }

                this.ExecuteAfterSeconds(UnityEngine.Random.Range(0.1f, 0.6f) / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                {
                    Bug();
                });
            });
        }
    }
}