using System;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.BulletMods
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

            Type type = typeof(GravityInvertShotsEffect);

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            GameObject obj = new GameObject("GravityInvertEffect", type);

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

        public Player owner;

        private MoveTransform moveTransform;

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

                moveTransform = GetComponent<MoveTransform>();
                while (owner == null) { owner = GetComponent<SpawnedAttack>().spawner; }

                if ( moveTransform != null)
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