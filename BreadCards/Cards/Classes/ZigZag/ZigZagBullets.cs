using BreadCards.Cards.Classes.ZigZag;
using ClassesManagerReborn;
using ClassesManagerReborn.Util;
using ModsPlus;
using Photon.Pun;
using RWF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace BreadCards.Cards
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

            GameObject obj = new GameObject("ZigZagShotss", typeof(ZigZagShots));

            ZigZagShots.ownerID = player.playerID;

            obj.GetComponent<ZigZagShots>();

            List<ObjectsToSpawn> list = gun.objectsToSpawn.ToList();
            list.Add(new ObjectsToSpawn
            {
                AddToProjectile = obj
            });
            gun.objectsToSpawn = list.ToArray();

            ZigZagShots.resetData();
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
            return BreadCards.ModInitials;
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

    public class ZigZagShots : MonoBehaviour
    {
        public static int ownerID;

        public Player owner;

        public static float startDelay;
        public static float delay;
        public static bool randomness;

        private MoveTransform moveTransform;
        private PhotonView photonView;

        public static void resetData()
        {
            startDelay = 0.3f;
            delay = 0.2f;
            randomness = false;
        }
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
            photonView = GetComponent<PhotonView>();
            moveTransform = GetComponent<MoveTransform>();

            if (owner == null) { owner = PlayerManager.instance.GetPlayerWithID(ownerID); return; }


            if (owner != null) timescale = owner.data.weaponHandler.gun.projectielSimulatonSpeed;

            this.ExecuteAfterSeconds(startDelay / timescale, () =>
            {

                angle = owner.data.weaponHandler.gun.spread * 360f;

                if (randomness)
                {
                    Spread();
                    return;
                }

                if (moveTransform.velocity.x < 0)
                {
                    moveTransform.velocity = BreadCards.RotatedBy(moveTransform.velocity, angle * -0.5f);
                }
                else
                {
                    moveTransform.velocity = BreadCards.RotatedBy(moveTransform.velocity, angle * 0.5f);
                }

                if (moveTransform.velocity.x < 0) ZigZag1();
                else ZigZag2();
            });
        }
        public void ZigZag1()
        {
            moveTransform.velocity = BreadCards.RotatedBy(moveTransform.velocity, angle);
            this.ExecuteAfterSeconds(delay/timescale, () =>
            {
                ZigZag2();
            });
        }

        public void ZigZag2()
        {
            moveTransform.velocity = BreadCards.RotatedBy(moveTransform.velocity, angle *-1);
            this.ExecuteAfterSeconds(delay/timescale, () =>
            {
                ZigZag1();
            });
        }

        public void Spread()
        {
            float rangle = UnityEngine.Random.Range(angle * -0.5f, angle * 0.5f);

            moveTransform.velocity = BreadCards.RotatedBy(moveTransform.velocity, rangle);
            this.ExecuteAfterSeconds(delay / timescale, () =>
            {
                Spread();
            });
        }
    }
}