using ModdingUtils.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace BreadCards.Cards
{
    class AmmoSavingTechnology : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            UnityEngine.Debug.Log($"[{BreadCards.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            player.transform.gameObject.AddComponent<AmmoSave>();
            gunAmmo.maxAmmo -= 2;
            gun.reloadTime += 5;

            UnityEngine.Debug.Log($"[{BreadCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gunAmmo.maxAmmo += 2;
            gun.reloadTime -= 5;
            if (player.transform.gameObject.AddComponent<AmmoSave>() != null)
            {
                Destroy(player.transform.gameObject.AddComponent<AmmoSave>());
            }
            UnityEngine.Debug.Log($"[{BreadCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }

        protected override string GetTitle()
        {
            return "Ammo Saving Tech";
        }
        protected override string GetDescription()
        {
            return "You only use 1 ammo no matter how many bullets";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Ammo Cost",
                    amount = "1",
                    simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Ammo",
                    amount = "-2",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload Speed",
                    amount = "+2s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
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
    public class AmmoSave : MonoBehaviour
    {

        private CharacterData data;
        private Player player;
        private Block block;
        private WeaponHandler weaponHandler;
        private Gun gun;

        public void Start()
        {
            this.data = this.gameObject.GetComponentInParent<CharacterData>();
        }
        private void Update()
        {
            if (!player)
            {
                if (!(data is null))
                {
                    player = data.player;
                    block = data.block;
                    weaponHandler = data.weaponHandler;
                    gun = weaponHandler.gun;
                    gun.ShootPojectileAction += OnShootProjectileAction;
                }

            }
        }
        public void OnShootProjectileAction(GameObject obj)
        {
            int CurrentAmmo = (int)this.gameObject.GetComponentInParent<GunAmmo>().GetFieldValue("___currentAmmo");
            this.gameObject.GetComponentInParent<GunAmmo>().SetFieldValue("___currentAmmo", CurrentAmmo - 5);
        }

        public void OnDestroy()
        {
            gun.ShootPojectileAction -= OnShootProjectileAction;
        }
    }
}