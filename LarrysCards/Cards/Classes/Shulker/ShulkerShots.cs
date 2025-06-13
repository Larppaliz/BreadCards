using ClassesManagerReborn;
using ClassesManagerReborn.Util;
using ModdingUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.Classes.Shulker
{
    class ShulkerShots : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>();
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 0.8f;
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            if (!ShulkerHoming.stats.ContainsKey(player.playerID)) ShulkerHoming.stats.Add(player.playerID, new ShulkerData().resetData());
            else ShulkerHoming.stats[player.playerID].resetData();

            Type type = typeof(ShulkerHoming);

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            GameObject obj = new GameObject("ShulkerShotEffect", type);

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
            return "Shulker Shots";
        }
        protected override string GetDescription()
        {
            return "Your bullets will go towards targets when their axis aligns";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.ShulkerShotsArt;
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
                    stat = "Shulks",
                    amount = "5",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Accel Rate",
                    amount = "101%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "DMG",
                    amount = "-20%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },

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

    class ShulkerClass : ClassHandler
    {
        internal static string name = "Shulker";

        public override IEnumerator Init()
        {
            while (!(ShulkerShots.CardInfo && ExtraShulks.CardInfo && FasterShulks.CardInfo)) yield return null;

            ClassesRegistry.Register(ShulkerShots.CardInfo, CardType.Entry);

            CardInfo entry = ShulkerShots.CardInfo;

            ClassesRegistry.Register(ExtraShulks.CardInfo, CardType.Card, entry);
            ClassesRegistry.Register(DoubleShulks.CardInfo, CardType.Card, entry, 2);
            ClassesRegistry.Register(FasterShulks.CardInfo, CardType.Card, entry, 3);

            ClassesRegistry.Register(StoppingShulks.CardInfo, CardType.Card, entry, 1);

            ClassesRegistry.Register(TeleportingShulks.CardInfo, CardType.Branch, entry, 1);
            ClassesRegistry.Register(LongerTP.CardInfo, CardType.Card, new[] { entry, TeleportingShulks.CardInfo }, 0);
            ClassesRegistry.Register(LessTPDelay.CardInfo, CardType.Card, new[] { entry, TeleportingShulks.CardInfo }, 0);
        }
    }

    public class ShulkerData
    {

        public int shulkCount;
        public float shulkRate;
        public float shulkStop;
        public float TPrange;
        public float TPdelay;
        public ShulkerData resetData()
        {
            shulkCount = 5;
            shulkRate = 1.01f;
            shulkStop = 0;
            TPrange = 0;
            TPdelay = 0;

            return this;
        }
    }
    public class ShulkerHoming : MonoBehaviour
    {
        private bool start;
        private int shulks = 0;

        public static Dictionary<int, ShulkerData> stats = new Dictionary<int, ShulkerData>();

        private MoveTransform moveTransform;

        Player owner;

        ShulkerData sData;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<ShulkerHoming>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<ShulkerHoming>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            if (GetComponent<SpawnedAttack>() == null) return;

            if (owner == null) { owner = GetComponent<SpawnedAttack>().spawner; this.ExecuteAfterSeconds(0.01f, () => Awake()); return; }
            moveTransform = GetComponent<MoveTransform>();
            sData = stats[owner.playerID];
            shulks = sData.shulkCount;

            ogGravity = moveTransform.gravity;

            this.ExecuteAfterSeconds(0.3f, () =>
            {
                start = true;


                this.ExecuteAfterSeconds(0.3f, () =>
                {
                    canHurtOwner = true;
                });
            });


        }

        bool canHurtOwner = false;
        int dir = -1;
        int oldDir = -1;
        public void Update()
        {
            if (!start) return;



                Player owner = PlayerManager.instance.GetClosestPlayer(transform.position);

                foreach (Player player in PlayerManager.instance.players)
                {
                    if (player.data.currentCards.Contains(ShulkerShots.CardInfo))
                    {
                        owner = player;
                    }
                }

                float maxSpeed = owner.data.weaponHandler.gun.projectileSpeed * 30f * owner.data.weaponHandler.gun.projectielSimulatonSpeed;

                if (shulks > 0)
                {
                    oldDir = dir;
                    foreach (var player in PlayerManager.instance.players.Where(PlayerStatus.PlayerAlive))
                    {
                        if (Vector2.Distance(transform.position, player.transform.position) <= 200f)
                        {
                            if (player != owner || canHurtOwner)
                            {
                                if (Math.Round(player.transform.position.x) == Math.Round(transform.position.x) && player.transform.position.y < transform.position.y)
                                {
                                    if (dir != 0)
                                    {
                                        dir = 0;
                                        moveTransform.gravity = 0f;
                                        moveTransform.velocity = new Vector2(0, maxSpeed * -1);
                                        moveTransform.velocity /= 10f;
                                    }
                                }
                                if (Math.Round(player.transform.position.x) == Math.Round(transform.position.x) && player.transform.position.y > transform.position.y)
                                {
                                    if (dir != 1)
                                    {
                                        dir = 1;
                                        moveTransform.gravity = 0f;
                                        moveTransform.velocity = new Vector2(0, maxSpeed);
                                        moveTransform.velocity /= 10f;
                                    }
                                }
                                if (Math.Round(player.transform.position.y) == Math.Round(transform.position.y) && player.transform.position.x < transform.position.x)
                                {
                                    if (dir != 2)
                                    {
                                        dir = 2;
                                        moveTransform.gravity = 0f;
                                        moveTransform.velocity = new Vector2(maxSpeed * -1, 0);
                                        moveTransform.velocity /= 10f;
                                    }
                                }
                                if (Math.Round(player.transform.position.y) == Math.Round(transform.position.y) && player.transform.position.x > transform.position.x)
                                {
                                    if (dir != 3)
                                    {
                                        dir = 3;
                                        moveTransform.gravity = 0f;
                                        moveTransform.velocity = new Vector2(maxSpeed, 0);
                                        moveTransform.velocity /= 10f;
                                    }
                                }
                            }
                        }
                    }

                    if (oldDir != dir)
                    {
                        shulks--;
                        if (sData.TPrange > 0)
                        {
                            this.ExecuteAfterSeconds(sData.TPdelay, () =>
                            {
                                if (sData.TPrange > 0) transform.position += moveTransform.velocity * 10f * sData.TPrange;
                            });
                        }

                        if (sData.shulkStop > 0)
                        {
                            shulkStopper(sData.shulkStop);
                        }
                    }

                }
                if (moveTransform.velocity.magnitude <= maxSpeed && dir > -1)
                {
                    moveTransform.velocity *= sData.shulkRate;
                }
        }

        Coroutine shulkStopCoroutine;

        public void shulkStopper(float time)
        {
            if (shulkStopCoroutine != null)
                StopCoroutine(shulkStopCoroutine);
            shulkStopCoroutine = StartCoroutine(ShulkStopCoroutine(time));
        }
        float ogGravity;
        IEnumerator ShulkStopCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            shulks = sData.shulkCount;
            dir = -1;
            moveTransform.gravity = ogGravity;
        }
    }
}
