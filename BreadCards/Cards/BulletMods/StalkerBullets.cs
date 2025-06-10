using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards.BulletMods
{
    class StalkerBullets : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.destroyBulletAfter += 999;

            GameObject obj = new GameObject("StaklerShotEffect", typeof(StalkerShotEffect));

            obj.GetComponent<StalkerShotEffect>();

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
            return "Stalker Bullets";
        }
        protected override string GetDescription()
        {
            return "Your bullets will home into targets while they aren't looking";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.StalkerArt;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return BreadCards.ModInitials;
        }
    }


    public class StalkerShotEffect : MonoBehaviour
    {

        public Player owner;

        bool start = false;

        private MoveTransform moveTransform;


        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<StalkerShotEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<StalkerShotEffect>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            this.ExecuteAfterSeconds(0.3f, () =>
            {

                moveTransform = GetComponent<MoveTransform>();
                start = true;

                this.ExecuteAfterSeconds(0.5f, () =>
                {
                    ownerDelay = false;
                });
            });

        }
        bool ownerDelay = true;
        Player target = null;
        public void Update()
        {

            if (GetComponent<SpawnedAttack>() != null) owner = GetComponent<SpawnedAttack>().spawner;

            if (owner == null) return;
            if (!start) return;


                if (target == null || target.data.dead)
                {
                    Player player = PlayerManager.instance.GetClosestPlayer(transform.position, true);

                    if (player != null && !player.data.dead)
                    {
                        if (player == owner && ownerDelay) return;

                        if (BreadCards.Distance(transform.position, player.transform.position) < 15f)
                        {
                            target = player;
                        }
                    }
                }
                else
                {
                    moveTransform.gravity = 0f;

                    Vector2 directionToTarget = target.transform.position - transform.position;
                    float dotProduct = Vector2.Dot(target.data.aimDirection.normalized, directionToTarget.normalized);
                    if (dotProduct > 0)
                    {
                        Vector2 vel = BreadCards.Normalize(target.transform.position - transform.position);
                        moveTransform.velocity += new Vector3(vel.x, vel.y, 0f);
                    }
                    else
                    {
                        if (moveTransform.velocity.magnitude > 1f)
                        {
                            moveTransform.velocity *= 0.95f;
                        }
                    }
                }

        }
    }
}