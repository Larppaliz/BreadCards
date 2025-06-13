using LarrysCards.Cards.General;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace LarrysCards.Cards.BulletMods
{
    class GrenadeBullets : CustomCard
    {
        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            CardInfo = cardInfo;
            cardInfo.allowMultiple = true;
            gun.damage = 0.85f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            ActivatorMono.Add(player, (obj, i) =>
            {
                MoveTransform moveTransform = obj.GetComponent<MoveTransform>();
                if (moveTransform != null)
                {
                    if (obj.GetComponent<SpawnedAttack>() == null) return;

                    Player owner = obj.GetComponent<SpawnedAttack>().spawner;

                    if (owner == null) return;

                    try
                    { LarrysCards.CreateWorkingExplosion(owner, moveTransform.transform.position); }
                    catch (System.Exception)
                    {

                    }

                    LarrysCards.print(obj.isFinalAction);

                    if (!obj.isFinalAction) return;

                    Destroy(moveTransform.transform.root.gameObject);
                }
            });

            Type type = typeof(GrenadeShots);

            float defaultTime = 1.5f;

            if (!GrenadeShots.time.ContainsKey(player.playerID)) GrenadeShots.time.Add(player.playerID, defaultTime);
            else GrenadeShots.time[player.playerID] /= 2f;

            foreach (ObjectsToSpawn ots in gun.objectsToSpawn)
            {
                if (ots.AddToProjectile.GetComponent(type) != null) return;
            }

            GrenadeShots.time[player.playerID] = defaultTime;

            GameObject obj = new GameObject("GrenadeEffect", type);

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
            return "Grenade Bullets";
        }
        protected override string GetDescription()
        {
            return "<color=#9aa355>TIMED</color>\n" + "Your bullets explode after 1.5s";
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
                    amount = "-15%",
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

    public class GrenadeShots : MonoBehaviour
    {

        public static Dictionary<int, float> time = new Dictionary<int, float>();

        public Player owner;

        private MoveTransform moveTransform;

        bool activated;
        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<GrenadeShots>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<GrenadeShots>().Where(bullet => bullet != this))
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
                try
                { LarrysCards.CreateWorkingExplosion(owner, moveTransform.transform.position); }
                catch (System.Exception)
                {

                }

                Destroy(moveTransform.transform.root.gameObject);
            });

        }
    }
}