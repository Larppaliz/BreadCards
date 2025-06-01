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
    class SplitShot : CustomCard
    {
        public static GameObject objectToSpawn = null;
        public static CardInfo CardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            GameObject obj = new GameObject("TestHomingEffect", typeof(SplittinRoundEffect));

            obj.GetComponent<SplittinRoundEffect>();

            SplittinRoundEffect.ownerID = player.playerID;

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
            return "Splitting rounds";
        }
        protected override string GetDescription()
        {
            return "Your bullets will split into 2 after 0.5s";
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
                    stat = "Split Delay",
                    amount = "0.5s",
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


    public class SplittinRoundEffect : MonoBehaviour
    {
        public static int ownerID;

        public Player owner;

        private MoveTransform moveTransform;
        private PhotonView photonView;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<SplittinRoundEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<SplittinRoundEffect>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            owner = PlayerManager.instance.GetPlayerWithID(ownerID);

            this.ExecuteAfterSeconds(0.5f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
            {
                Explode();
            });

            photonView = GetComponent<PhotonView>();
            moveTransform = GetComponent<MoveTransform>();
        }
        public void Explode()
        {

            if (owner == null) { owner = PlayerManager.instance.GetPlayerWithID(ownerID); return; }

            if (photonView != null)
            {
                

                Vector2 vel = new Vector2(0, 2f);

                int bulletAmount = 2;

                Gun gun = owner.data.weaponHandler.gun;
                SimulatedGun sgun = new GameObject("ClusterGun").AddComponent<SimulatedGun>();

                sgun.CopyGunStatsExceptActions(gun);

                sgun.shootPosition = transform;


                ObjectsToSpawn[] list = new ObjectsToSpawn[0];

                foreach (ObjectsToSpawn obj in gun.objectsToSpawn)
                {
                    if (!obj.AddToProjectile.GetComponent<SplittinRoundEffect>())
                    {
                        list.AddItem(obj);
                    }
                }
                sgun.objectsToSpawn = list;
                sgun.numberOfProjectiles = 1;

                Vector2 angle = BreadCards.RotatedBy(moveTransform.velocity, -18f);

                sgun.SimulatedAttack(owner.playerID, transform.position, angle, 1f, 1f);

                angle = BreadCards.RotatedBy(moveTransform.velocity, 18f);

                sgun.SimulatedAttack(owner.playerID, transform.position, angle, 1f, 1f);


                foreach (var obj in GetComponentsInChildren<SplittinRoundEffect>().Where(bullet => bullet == this))
                {
                    Destroy(obj.gameObject);
                }
            }
        }
    }
}