using LarrysCards.Cards.General;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.BulletMods
{
    class LaunchingBullets : CustomCard
    {
        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            CardInfo = cardInfo;
            cardInfo.allowMultiple = true;
            gun.damage = 0.75f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            ActivatorMono.Add(player, (obj, i) =>
            {
                MoveTransform moveTransform = obj.GetComponent<MoveTransform>();
                if (moveTransform != null)
                {

                    Player player = PlayerManager.instance.GetClosestPlayer(moveTransform.transform.position, false);

                    if (player != null)
                    {
                        
                        float mag = moveTransform.velocity.magnitude;

                        moveTransform.gravity = 0f;
                        Vector2 vel = (player.transform.position - moveTransform.transform.position).normalized;
                        moveTransform.velocity = new Vector3(vel.x, vel.y, 0f) * mag;
                    }
                }
            });

            Type type = typeof(LaunchingShots);

            float defaultTime = 1f;

            if (!LaunchingShots.time.ContainsKey(player.playerID)) LaunchingShots.time.Add(player.playerID, defaultTime);
            else LaunchingShots.time[player.playerID] /= 2f;

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            LaunchingShots.time[player.playerID] = defaultTime;

            GameObject obj = new GameObject("LaunchingEffect", type);

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
            return "Launching Bullets";
        }
        protected override string GetDescription()
        {
            return "<color=#9aa355>TIMED</color>\n" + "Your bullets launch at nearby targets after 1s";
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
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.Some
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.FirepowerYellow;
        }
        public override string GetModName()
        {
            return LarrysCards.ModInitials;
        }
    }

    public class LaunchingShots : MonoBehaviour
    {

        public static Dictionary<int, float> time = new Dictionary<int, float>();

        public Player owner;

        private MoveTransform moveTransform;

        bool activated;
        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<LaunchingShots>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<LaunchingShots>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            if (GetComponent<SpawnedAttack>() == null) return;

            owner = GetComponent<SpawnedAttack>().spawner;

            if (owner == null) { this.ExecuteAfterFrames(1, () => { Awake(); }); return; }

            activated = owner.GetComponent<ActivatorMono>().actionsEnabled;

            if (activated) return;

            int ownerID = owner.playerID;

            print(ownerID);

            float newtime = time[ownerID];

            print(newtime);

            moveTransform = GetComponent<MoveTransform>();

            if (moveTransform != null)
            {

                this.ExecuteAfterSeconds(newtime / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                {
                    Player player = PlayerManager.instance.GetClosestPlayer(transform.position, false);

                    if (player != null)
                    {
                        float mag = moveTransform.velocity.magnitude;

                        moveTransform.gravity = 0f;
                        Vector2 vel = (player.transform.position - moveTransform.transform.position).normalized;
                        moveTransform.velocity = new Vector3(vel.x, vel.y, 0f) * mag;

                    }
                });
            }
        }
    }
}