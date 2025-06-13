using ClassesManagerReborn;
using ModdingUtils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.Classes.Magnet
{
    class MagnetShots : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 0.75f;
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            if (!MagnetShot.stats.ContainsKey(player.playerID)) MagnetShot.stats.Add(player.playerID, new MagnetData().resetData());
            else MagnetShot.stats[player.playerID].resetData();

                Type type = typeof(MagnetShot);

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            GameObject obj = new GameObject("MagnetShotEffect", type);

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
            return "Magnet Shots";
        }
        protected override string GetDescription()
        {
            return "Makes your bullets change direction towards <color=#ff0000>Enemies</color> when they come close to them";
        }
        protected override GameObject GetCardArt()
        {
            return Assets.MagnetShotsArt;
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
                    stat = "Magnet Range",
                    amount = "5m",
                    simepleAmount = CardInfoStat.SimpleAmount.Some
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Magnet Delay",
                    amount = "0.2s",
                    simepleAmount = CardInfoStat.SimpleAmount.Some
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Magnet CD",
                    amount = "1s",
                    simepleAmount = CardInfoStat.SimpleAmount.Some
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "DMG",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
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

    class MagnetClass : ClassHandler
    {
        internal static string name = "Magnet";

        public override IEnumerator Init()
        {
            while (!(MagnetShots.CardInfo && LessMagnetDelay.CardInfo)) yield return null;
            ClassesRegistry.Register(MagnetShots.CardInfo, CardType.Entry);
            ClassesRegistry.Register(LessMagnetDelay.CardInfo, CardType.Card, MagnetShots.CardInfo, 0);
            ClassesRegistry.Register(MoreMagnetRange.CardInfo, CardType.Card, MagnetShots.CardInfo, 0);
            ClassesRegistry.Register(LessMagnetCooldown.CardInfo, CardType.Card, MagnetShots.CardInfo, 0);
            ClassesRegistry.Register(MagnetTeleport.CardInfo, CardType.Branch, MagnetShots.CardInfo, 1);
            ClassesRegistry.Register(MagnetStop.CardInfo, CardType.Card, new CardInfo[] { MagnetShots.CardInfo, MagnetTeleport.CardInfo }, 1) ;
        }
    }

    public class MagnetData
    {
        public float magnetDelay;
        public float magnetRange;
        public float magnetCD;
        public bool magnetTP;
        public bool magnetStop;

        public MagnetData resetData()
        {
            magnetDelay = 0.2f;
            magnetRange = 6f;
            magnetCD = 1f;
            magnetTP = false;
            magnetStop = false;

            return this;
        }
    }
    public class MagnetShot : MonoBehaviour
    {

        public Player owner;

        public static Dictionary<int, MagnetData> stats = new Dictionary<int, MagnetData>();

        private MoveTransform moveTransform;

        MagnetData mData;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<MagnetShot>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<MagnetShot>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            if (owner == null && GetComponent<SpawnedAttack>() != null) { owner = GetComponent<SpawnedAttack>().spawner; this.ExecuteAfterSeconds(0.01f, () => Awake()); return; }
            this.ExecuteAfterSeconds(0.05f, () =>
            {
                delay = false;
                start = true;
                ownerDelay = true;
                moveTransform = GetComponent<MoveTransform>();

                this.ExecuteAfterSeconds(0.5f, () =>
                {
                    ownerDelay = false;
                });
                });

        }
        bool start = false;
        bool delay;
        bool ownerDelay;
        public void Update()
        {
            if (GetComponent<SpawnedAttack>() == null || moveTransform == null) return;


                if (owner == null) { owner = GetComponent<SpawnedAttack>().spawner; return; }

                mData = stats[owner.playerID];

                if (start)
                {

                    foreach (var player in PlayerManager.instance.players.Where(PlayerStatus.PlayerAlive))
                    {
                        if (Vector2.Distance(transform.position, player.transform.position) < mData.magnetRange && !delay)
                        {
                            if (player != owner || !ownerDelay) magnetize(player.transform.position);
                        }
                    }
                }

        }

        public void magnetize(Vector3 targetPos)
        {
            if (start)
            {
                delay = true;
                float grav = moveTransform.gravity;
                moveTransform.velocity = new Vector2(0.0f, 0.01f);
                moveTransform.gravity *= 0f;
                this.ExecuteAfterSeconds(mData.magnetDelay, () =>
                {

                    moveTransform.velocity = (transform.position - targetPos).normalized * 80f;

                    if (mData.magnetTP) transform.position = targetPos;

                    if (mData.magnetStop) { moveTransform.velocity = new Vector2(0.0f, 0.01f); moveTransform.gravity *= 0f; }

                    this.ExecuteAfterSeconds(0.4f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                    {
                        moveTransform.gravity = grav;
                    });

                    this.ExecuteAfterSeconds(mData.magnetCD, () =>
                    {
                        delay = false;
                    });
                });
            }
        }
    }
}