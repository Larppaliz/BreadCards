using System;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.BulletMods
{
    class BadHoming : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Type type = typeof(BadHomingEffect);

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            GameObject obj = new GameObject("BadHomingEffect", type);

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
            return "Homer bullet";
        }
        protected override string GetDescription()
        {
            return "Your bullets will attempt to home into <color=#ff0000>Enemies</color>...\n Doesn't do that well on its own.";
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
            return LarrysCards.ModInitials;
        }
    }


    public class BadHomingEffect : MonoBehaviour
    {

        public Player owner;

        bool start = false;
        bool ownerDelay = true;

        private MoveTransform moveTransform;

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
            if (owner == null && GetComponent<SpawnedAttack>() != null) { owner = GetComponent<SpawnedAttack>().spawner; this.ExecuteAfterSeconds(0.01f, () => Awake()); return; }
            this.ExecuteAfterSeconds(0.4f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
            {
                start = true;
                this.ExecuteAfterSeconds(0.5f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                {
                    ownerDelay = false;
                });
            });

            moveTransform = GetComponent<MoveTransform>();
        }
        bool ownerdelay = true;
        public void Update()
        {

            if (GetComponent<SpawnedAttack>() != null) owner = GetComponent<SpawnedAttack>().spawner;

            if (owner == null) return;

            if (!start) return;


                Player player = PlayerManager.instance.GetClosestPlayer(transform.position, true);

                if (player != null)
                {
                    if (player == owner && ownerdelay) return;

                Vector2 targetpos = player.transform.position;
                    if (Vector2.Distance(transform.position, targetpos) < 20f)
                    {
                        Vector2 vel = (player.transform.position - transform.position).normalized;
                        moveTransform.velocity += new Vector3(vel.x, vel.y, 0f) * owner.data.weaponHandler.gun.projectielSimulatonSpeed;
                    }
                }

        }
    }
}