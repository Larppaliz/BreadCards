using LarrysCards.Cards.General;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace LarrysCards.Cards.BulletMods
{
    class GravityInverterBullets : CustomCard
    {
        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            CardInfo = cardInfo;
            cardInfo.allowMultiple = true;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            ActivatorMono.Add(player, (obj, i) =>
            {
                MoveTransform moveTransform = obj.GetComponent<MoveTransform>();
                if (moveTransform != null)
                {
                    moveTransform.gravity *= -1f;
                }
            });

            Type type = typeof(GravityInverterShots);

            float defaultTime = 0.4f;

            if (!GravityInverterShots.time.ContainsKey(player.playerID)) GravityInverterShots.time.Add(player.playerID, defaultTime);
            else GravityInverterShots.time[player.playerID] /= 2f;

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            GravityInverterShots.time[player.playerID] = defaultTime;

            GameObject obj = new GameObject("GravityInverterEffect", type);

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
            return "Gravity Flipper Bullets";
        }
        protected override string GetDescription()
        {
            return "<color=#9aa355>TIMED</color>\n" + "Your bullets flip gravity every 0.4s";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
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

    public class GravityInverterShots : MonoBehaviour
    {

        public static Dictionary<int, float> time = new Dictionary<int, float>();

        public Player owner;

        private MoveTransform moveTransform;

        bool activated;
        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<GravityInverterShots>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<GravityInverterShots>().Where(bullet => bullet != this))
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

            if (moveTransform == null) return;

            this.ExecuteAfterSeconds(newtime / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
            {
                moveTransform.gravity *= -1f;
                Invert(newtime);
            });

        }

        public void Invert(float newtime)
        {
            this.ExecuteAfterSeconds(newtime / owner.data.weaponHandler.gun.projectielSimulatonSpeed, () =>
            {
                Invert(newtime);
                moveTransform.gravity *= -1f;
            });
        }

    }
}