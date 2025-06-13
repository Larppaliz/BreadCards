using ClassesManagerReborn;
using ClassesManagerReborn.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.Classes.ZigZag
{
    class ZigZagBullets : CustomCard
    {
        public static CardInfo CardInfo;

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>();
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 1.25f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.spread += 45f / 360f;

            gun.gravity = 0f;

            if (!ZigZagShots.stats.ContainsKey(player.playerID)) ZigZagShots.stats.Add(player.playerID, new ZigZagData().resetData());
            else ZigZagShots.stats[player.playerID].resetData();

            Type type = typeof(ZigZagShots);

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            GameObject obj = new GameObject("ZigZagShotEffect", type);

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
            return "Zig Zag Bullets";
        }
        protected override string GetDescription()
        {
            return "Your bullets will Zig Zag & the angle is based on your spread";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.ZigZagArt;
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
                    stat = "Zig Zag Delay",
                    amount = "0.2s",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Start Delay",
                    amount = "0.3s",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "DMG",
                    amount = "+25%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullet Gravity",
                    amount = "0",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Spread",
                    amount = "+45°",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
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
    
    class ZigZagsClass : ClassHandler
    {
        internal static string name = "ZigZag";
      
        public override IEnumerator Init()
        {
            while (!(ZigZagBullets.CardInfo && FasterZigZag.CardInfo && SlowerZigZag.CardInfo)) yield return null;


            ClassesRegistry.Register(ZigZagBullets.CardInfo, CardType.Entry);

            CardInfo entry = ZigZagBullets.CardInfo;


            ClassesRegistry.Register(FasterZigZag.CardInfo, CardType.Card, entry);
            ClassesRegistry.Register(SlowerZigZag.CardInfo, CardType.Card, entry);
            ClassesRegistry.Register(LightspeedZigZag.CardInfo, CardType.Card, new CardInfo[] {entry, FasterZigZag.CardInfo}, 1);
            ClassesRegistry.Register(RandomZigZag.CardInfo, CardType.Card, entry, 1);
        }
    }

    public class ZigZagData
    {
        public float startDelay;
        public float delay;
        public bool randomness;

        public ZigZagData resetData()
        {
            startDelay = 0.3f;
            delay = 0.2f;
            randomness = false;

            return this;
        }
    }
    public class ZigZagShots : MonoBehaviour
    {

        public Player owner;

        public static Dictionary<int, ZigZagData> stats = new Dictionary<int, ZigZagData>();
        ZigZagData zData;

        private MoveTransform moveTransform;


        public float angle;
        float timescale = 1f;
        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<ZigZagShots>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<ZigZagShots>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }

            moveTransform = GetComponent<MoveTransform>();

            if (owner == null && GetComponent<SpawnedAttack>() != null) { owner = GetComponent<SpawnedAttack>().spawner; return; }

            zData = stats[owner.playerID];

            if (owner != null) timescale = owner.data.weaponHandler.gun.projectielSimulatonSpeed;

            this.ExecuteAfterSeconds(zData.startDelay / timescale, () =>
            {

                angle = owner.data.weaponHandler.gun.spread * 360f;

                if (zData.randomness)
                {
                    Spread();
                    return;
                }

                if (moveTransform.velocity.x < 0)
                {
                    moveTransform.velocity = LarrysCards.RotatedBy(moveTransform.velocity, angle * -0.5f);
                }
                else
                {
                    moveTransform.velocity = LarrysCards.RotatedBy(moveTransform.velocity, angle * 0.5f);
                }

                if (moveTransform.velocity.x < 0) ZigZag1();
                else ZigZag2();
            });
        }
        public void ZigZag1()
        {
            moveTransform.velocity = LarrysCards.RotatedBy(moveTransform.velocity, angle);
            this.ExecuteAfterSeconds(zData.delay /timescale, () =>
            {
                ZigZag2();
            });
        }

        public void ZigZag2()
        {
            moveTransform.velocity = LarrysCards.RotatedBy(moveTransform.velocity, angle *-1);
            this.ExecuteAfterSeconds(zData.delay /timescale, () =>
            {
                ZigZag1();
            });
        }

        public void Spread()
        {
            float rangle = UnityEngine.Random.Range(angle * -0.5f, angle * 0.5f);

            moveTransform.velocity = LarrysCards.RotatedBy(moveTransform.velocity, rangle);
            this.ExecuteAfterSeconds(zData.delay / timescale, () =>
            {
                Spread();
            });
        }
    }
}