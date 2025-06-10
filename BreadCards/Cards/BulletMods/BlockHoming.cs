using System;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.BulletMods
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

            Type type = typeof(BlockHomingEffect);

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            GameObject obj = new GameObject("BlockHomingEffect", type);

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

        public Player owner;

        private MoveTransform moveTransform;
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
                moveTransform = GetComponent<MoveTransform>();
            });
        }
        bool start;
        public void Update()
        {
            if (GetComponent<SpawnedAttack>() != null) owner = GetComponent<SpawnedAttack>().spawner;

            if (owner == null) return;
            if (!start) return;



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