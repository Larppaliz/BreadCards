using ModsPlus;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using SimulationChamber;
using HarmonyLib;
using System;

namespace BreadCards.Cards
{
    class ProximityClusterBomb : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            gun.damage = 0.3f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            GameObject obj = new GameObject("TestHomingEffect", typeof(ProximityClusterBombEffect));

            obj.GetComponent<ProximityClusterBombEffect>();

            ProximityClusterBombEffect.ownerID = player.playerID;

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
            return "Proximity Cluster Bomb Rounds";
        }
        protected override string GetDescription()
        {
            return "Your bullets will explode into 6 to 12 bullets when near targets";
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
                    amount = "-70%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                                new CardInfoStat()
                {
                    positive = false,
                    stat = "Cluster Range",
                    amount = "-85%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                }
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


    public class ProximityClusterBombEffect : MonoBehaviour
    {
        public static int ownerID;

        public Player owner;

        private MoveTransform moveTransform;
        private PhotonView photonView;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<ProximityClusterBombEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<ProximityClusterBombEffect>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            this.ExecuteAfterSeconds(0.2f, () =>
            {
                owner = PlayerManager.instance.GetPlayerWithID(ownerID);
                photonView = GetComponent<PhotonView>();
                moveTransform = GetComponent<MoveTransform>();
                start = true;

                this.ExecuteAfterSeconds(0.5f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
                {
                    ownerDelay = false;
                });
            });
        }

        bool ownerDelay = true;
        public void Update()
        {
            if (!start) return;

            if (owner == null) { owner = PlayerManager.instance.GetPlayerWithID(ownerID); return; }

            if (photonView != null)
            {
                Player player = PlayerManager.instance.GetClosestPlayer(transform.position, true);

                if (player != null && !player.data.dead)
                {
                    if (player == owner && ownerDelay) return;

                    if (BreadCards.Distance(transform.position, player.transform.position) < 7.5f)
                    {
                        Explode(BreadCards.DirectionTo(transform.position, player.transform.position) * 2f);
                    }
                }

            }
        }
        bool start;
        public void Explode(Vector2 vel)
        {

            if (owner == null) { owner = PlayerManager.instance.GetPlayerWithID(ownerID); return; }

            if (photonView != null)
            {

                int bulletAmount = UnityEngine.Random.Range(6,12);

                Gun gun = owner.data.weaponHandler.gun;
                SimulatedGun sgun = new GameObject("ClusterGun").AddComponent<SimulatedGun>();

                sgun.CopyGunStatsExceptActions(gun);

                sgun.shootPosition = transform;


                ObjectsToSpawn[] list = new ObjectsToSpawn[0];

                foreach (ObjectsToSpawn obj in gun.objectsToSpawn)
                {
                    if (!obj.AddToProjectile.GetComponent<ProximityClusterBombEffect>())
                    {
                        list.AddItem(obj);
                    }
                }
                sgun.objectsToSpawn = list;
                sgun.numberOfProjectiles = 1;
                sgun.destroyBulletAfter *= 0.15f;

                for (int i = 0; i < bulletAmount; i++)
                {
                    Vector2 angle = BreadCards.RotatedBy(vel, 360/bulletAmount * i);

                    sgun.SimulatedAttack(owner.playerID, transform.position, angle, 1f, 1f);
                }


                foreach (var obj in GetComponentsInChildren<ProximityClusterBombEffect>().Where(bullet => bullet == this))
                {
                    Destroy(obj.gameObject);
                }
            }
        }
    }
}