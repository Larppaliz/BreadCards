using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using SimulationChamber;
using HarmonyLib;
using System;
using Photon.Pun;

namespace BreadCards.Cards.BulletMods
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
            Type type = typeof(SplittinRoundEffect);

            int defaultBullets = 2;

            if (!SplittinRoundEffect.bulletAmounts.ContainsKey(player.playerID)) SplittinRoundEffect.bulletAmounts.Add(player.playerID, defaultBullets);
            else SplittinRoundEffect.bulletAmounts[player.playerID]++;

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            SplittinRoundEffect.bulletAmounts[player.playerID] = defaultBullets;

            GameObject obj = new GameObject("SplittinRoundEffect", type);

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

        public Player owner;

        public static Dictionary<int, int> bulletAmounts = new Dictionary<int, int>();

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
            if (owner == null && GetComponent<SpawnedAttack>() != null) { owner = GetComponent<SpawnedAttack>().spawner; this.ExecuteAfterSeconds(0.01f, () => Awake()); return; }

            photonView = GetComponent<PhotonView>();
            moveTransform = GetComponent<MoveTransform>();

            this.ExecuteAfterSeconds(0.5f / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
            {
                Explode();
            });

        }
        public void Explode()
        {

            if (owner == null && GetComponent<SpawnedAttack>() != null) { owner = GetComponent<SpawnedAttack>().spawner; return; }

            if (photonView != null)
            {
                

                Vector2 vel = new Vector2(0, 2f);

                int bulletAmount = bulletAmounts[owner.playerID];

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

                float maxAngle = 45f;

                for (int i = 0; i < bulletAmount; i++)
                {
                    float angleOffset = Mathf.Lerp(-maxAngle/2, maxAngle/2, (float)i / (bulletAmount - 1));

                    Vector2 angle = BreadCards.RotatedBy(moveTransform.velocity, angleOffset);

                    sgun.SimulatedAttack(owner.playerID, transform.position, angle, 1f, 1f);

                }


                foreach (var obj in GetComponentsInChildren<SplittinRoundEffect>().Where(bullet => bullet == this))
                {
                    Destroy(obj.gameObject);
                }
            }
        }
    }
}