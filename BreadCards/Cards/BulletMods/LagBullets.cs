using Photon.Pun;
using System;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.BulletMods
{
    class LagBullets : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;

            gun.projectielSimulatonSpeed = 0.75f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Type type = typeof(LagBulletsEffect);

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            GameObject obj = new GameObject("LagEffect", type);

            gun.objectsToSpawn = gun.objectsToSpawn.Append(new ObjectsToSpawn
            {
                AddToProjectile = obj
            }).ToArray();

        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

        }
        protected override string GetTitle()
        {
            return "Laggy Bullets";
        }
        protected override string GetDescription()
        {
            return "Your bullets will sometimes lag backwards";
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
                    stat = "Bullet Timescale",
                    amount = "-25%",
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


    public class LagBulletsEffect : MonoBehaviour
    {

        public Player owner;

        bool start;

        private MoveTransform moveTransform;
        private PhotonView photonView;
        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<LagBulletsEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<LagBulletsEffect>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            if (owner == null && GetComponent<SpawnedAttack>() != null) { owner = GetComponent<SpawnedAttack>().spawner; this.ExecuteAfterSeconds(0.01f, () => Awake()); return; }

            this.ExecuteAfterSeconds(0.01f, () =>
            {
                start = true;
                photonView = GetComponent<PhotonView>();
                moveTransform = GetComponent<MoveTransform>();

                if (photonView != null && moveTransform != null)
                {
                    this.ExecuteAfterSeconds(0.2f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                    {
                        Lag();
                    });
                }
            });
        }
        public void Update()
        {
        }
        Vector2 oldPos;
        Vector2 oldVel;
        public void Lag()
        {

            oldPos = transform.position;
            oldVel = moveTransform.velocity;
            this.ExecuteAfterSeconds(UnityEngine.Random.Range(0.1f, 1.2f) / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
            {
                moveTransform.velocity = oldVel;
                transform.position = oldPos;
                this.ExecuteAfterSeconds(UnityEngine.Random.Range(0.1f, 0.6f) / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                {
                    Lag();
                });
            });
        }
    }
}