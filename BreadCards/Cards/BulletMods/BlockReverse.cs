using System;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.BulletMods
{
    class BlockReverse : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;

            block.cdMultiplier = 0.75f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Type type = typeof(BlockReverseEffect);

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            GameObject obj = new GameObject("BlockReverseEffect", type);

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
            return "Activated Reversing bullets";
        }
        protected override string GetDescription()
        {
            return "Your bullets will reverse direction when you block";
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


    public class BlockReverseEffect : MonoBehaviour
    {
        public Player owner;

        bool start;

        private MoveTransform moveTransform;
        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<BlockReverseEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<BlockReverseEffect>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }


            this.ExecuteAfterSeconds(0.01f, () =>
            {
                start = true;
                moveTransform = GetComponent<MoveTransform>();
            });
        }
        public void Update()
        {
            if (GetComponent<SpawnedAttack>() != null) owner = GetComponent<SpawnedAttack>().spawner;

            if (owner == null) return;

            if (!start) return;



                if (owner.data.block.sinceBlock == 0)
                {
                    moveTransform.velocity *= -1f;
                }

        }
    }
}