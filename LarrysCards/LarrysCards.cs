using BepInEx;
using LarrysCards.Cards.BulletMods;
using LarrysCards.Cards.Classes.Magnet;
using LarrysCards.Cards.Classes.Shulker;
using LarrysCards.Cards.Debuff;
using LarrysCards.Cards.General;
using ClassesManagerReborn;
using HarmonyLib;
using ModdingUtils.Utils;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnboundLib.Utils;
using UnityEngine;
using LarrysCards.Patches;
using SoundImplementation;
using Sonigon;

namespace LarrysCards
{
    // These are the mods required for our mod to work
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.classes.manager.reborn", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("larppaliz.rounds.settingsmod", BepInDependency.DependencyFlags.HardDependency)]
    // Declares our mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our mod is associated with
    [BepInProcess("Rounds.exe")]
    public class LarrysCards : BaseUnityPlugin
    {
        public static LarrysCards instance { get; private set; }

        private const string ModId = "larppaliz.rounds.larryscards";
        private const string ModName = "Larrys's Cards Remake";
        public const string Version = "0.2.0"; // What version are we on (major.minor.patch)?
        public const string ModInitials = "LCrm";

        public GameObject optionsMenu;

        void Awake()
        {
            instance = this;

            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
            GameModeManager.AddHook(GameModeHooks.HookPickStart, PickStart);
            GameModeManager.AddHook(GameModeHooks.HookRoundEnd, RoundEnd);
            GameModeManager.AddHook(GameModeHooks.HookGameEnd, ResetCardChoiceStuff);
            GameModeManager.AddHook(GameModeHooks.HookGameStart, ResetCardChoiceStuff);
            GameModeManager.AddHook(GameModeHooks.HookPickEnd, ResetCardChoiceOverrideSlots);

        }
        public static List<CardInfo> MyCursedCards = new List<CardInfo>();
        void Start()
        {


            CustomCard.BuildCard<DoubleShot>();
            CustomCard.BuildCard<BlockingMaster>();
            CustomCard.BuildCard<Rebuild>();
            CustomCard.BuildCard<FocusedShots>();
            CustomCard.BuildCard<SprayAndPray>();
            CustomCard.BuildCard<SuperBurst>();
            CustomCard.BuildCard<BulletStream>();
            CustomCard.BuildCard<SuperShotgun>();
            CustomCard.BuildCard<QuickswitchMagazine>();
            CustomCard.BuildCard<AnvilCard>();
            CustomCard.BuildCard<TimelessBullets>();
            CustomCard.BuildCard<DualBurst>();

            CustomCard.BuildCard<IWantOneToo>();
            CustomCard.BuildCard<CardDealer>();
            CustomCard.BuildCard<GiveItToMeLater>(c => GiveItToMeLater.CardInfo = c);
            CustomCard.BuildCard<DoubleIt>(c => DoubleIt.CardInfo = c);
            CustomCard.BuildCard<StaticBullets>(c => StaticBullets.CardInfo = c);
            CustomCard.BuildCard<LaunchingBullets>(c => StaticBullets.CardInfo = c);
            CustomCard.BuildCard<ReversingBullets>(c => ReversingBullets.CardInfo = c);
            CustomCard.BuildCard<GrenadeBullets>(c => GrenadeBullets.CardInfo = c);

            CustomCard.BuildCard<FuseBullets>();
            CustomCard.BuildCard<BouncesToBullets>();
            CustomCard.BuildCard<ExtraDamage>();
            CustomCard.BuildCard<LightSpeedBuckshot>();
            CustomCard.BuildCard<BarrelMod>();
            CustomCard.BuildCard<MoonBullets>();

            CustomCard.BuildCard<ActivatorCard>(c => { ActivatorCard.CardInfo = c; });

            CustomCard.BuildCard<AccelleratingBullets>(c => { AccelleratingBullets.CardInfo = c; });
            CustomCard.BuildCard<GravityInverterBullets>(c => { GravityInverterBullets.CardInfo = c; });
            CustomCard.BuildCard<BadHoming>(c => { BadHoming.CardInfo = c; });

            CustomCard.BuildCard<SplitBullets>(c => { SplitBullets.CardInfo = c; });
            //CustomCard.BuildCard<ClusterBullets>(c => { ClusterBullets.CardInfo = c; });

            CustomCard.BuildCard<HoppingBullets>(c => { HoppingBullets.CardInfo = c; });
            CustomCard.BuildCard<PlayerControlledBullets>(c => { PlayerControlledBullets.CardInfo = c; });
            CustomCard.BuildCard<StalkerBullets>(c => { StalkerBullets.CardInfo = c; });
            CustomCard.BuildCard<LagBullets>(c => { LagBullets.CardInfo = c; });
            CustomCard.BuildCard<BugBullets>(c => { BugBullets.CardInfo = c; });

            // CustomCard.BuildCard<ProximityFuse>((card) => { ProximityFuse.CardInfo = card; });

            CustomCard.BuildCard<CardTricks>();
            CustomCard.BuildCard<Copier>(c => Copier.CardInfo = c);
            CustomCard.BuildCard<OldCopier>(c => OldCopier.CardInfo = c);

            //CustomCard.BuildCard<CardUpgrade>();

            CustomCard.BuildCard<UltraDefense>(c =>  UltraDefense.CardInfo = c);
            CustomCard.BuildCard<MultiplyingSlime>(c => MultiplyingSlime.CardInfo = c);

            CustomCard.BuildCard<BlackMarket>(c => BlackMarket.CardInfo = c);
            CustomCard.BuildCard<RiceMarket>(c => RiceMarket.CardInfo = c);
            //CustomCard.BuildCard<BoosterPack>();


            CustomCard.BuildCard<DarkWebDeals>(c =>  DarkWebDeals.CardInfo = c );
            CustomCard.BuildCard<UltimateCopyCat>(c =>  UltimateCopyCat.CardInfo = c );
            CustomCard.BuildCard<Rice>(c => { Rice.CardInfo = c; });

            CustomCard.BuildCard<BlackMarketCopy>(c => { BlackMarketCopy.CardInfo = c; });

            CustomCard.BuildCard<UltraDefenseCopy>(c => { UltraDefenseCopy.CardInfo = c; });
            CustomCard.BuildCard<MultipliedSlime>(c => { MultipliedSlime.CardInfo = c; });

            //Debuff givers
            CustomCard.BuildCard<TrueEvil>(c => TrueEvil.CardInfo = c);
            CustomCard.BuildCard<PlotArmor>(c => PlotArmor.CardInfo = c);
            CustomCard.BuildCard<BigMonkeAttack>(c => BigMonkeAttack.CardInfo = c);
            CustomCard.BuildCard<BounceSabotage>(c => BounceSabotage.CardInfo = c);
            CustomCard.BuildCard<StickySabotage>(c => StickySabotage.CardInfo = c);

            //Debuff Cards
            CustomCard.BuildCard<EvilCurse>(card => { EvilCurse.CardInfo = card; });
            CustomCard.BuildCard<BigMonkeCurse>(card => { BigMonkeCurse.CardInfo = card; });
            CustomCard.BuildCard<CurseofTheTroopers>(card => { CurseofTheTroopers.CardInfo = card; });
            CustomCard.BuildCard<BounceCurse>(card => { BounceCurse.CardInfo = card; });
            CustomCard.BuildCard<StickySolution>(card => { StickySolution.CardInfo = card; });


            //Shulker Class
            CustomCard.BuildCard<ShulkerShots>((card) => { ShulkerShots.CardInfo = card;; });
            CustomCard.BuildCard<ExtraShulks>((card) => { ExtraShulks.CardInfo = card;});
            CustomCard.BuildCard<DoubleShulks>((card) => { DoubleShulks.CardInfo = card; });
            CustomCard.BuildCard<FasterShulks>((card) => { FasterShulks.CardInfo = card;});
            CustomCard.BuildCard<StoppingShulks>((card) => { StoppingShulks.CardInfo = card; });

            CustomCard.BuildCard<TeleportingShulks>((card) => { TeleportingShulks.CardInfo = card; });
            CustomCard.BuildCard<LongerTP>((card) => { LongerTP.CardInfo = card; });
            CustomCard.BuildCard<LessTPDelay>((card) => { LessTPDelay.CardInfo = card; });

            //Magnet Class
            CustomCard.BuildCard<MagnetShots>(card=> { MagnetShots.CardInfo = card; });

            CustomCard.BuildCard<LessMagnetDelay>(card=> { LessMagnetDelay.CardInfo = card; });
            CustomCard.BuildCard<LessMagnetCooldown>(card=> { LessMagnetCooldown.CardInfo = card; });
            CustomCard.BuildCard<MoreMagnetRange>(card=> { MoreMagnetRange.CardInfo = card; });
            CustomCard.BuildCard<MagnetTeleport>(card=> { MagnetTeleport.CardInfo = card; });
            CustomCard.BuildCard<MagnetStop>(card=> { MagnetStop.CardInfo = card; });

            /*
            CustomCard.BuildCard<ZigZagBullets>(card=> { ZigZagBullets.CardInfo = card; });

            CustomCard.BuildCard<FasterZigZag>(card=> { FasterZigZag.CardInfo = card; });
            CustomCard.BuildCard<SlowerZigZag>(card=> { SlowerZigZag.CardInfo = card; });
            CustomCard.BuildCard<LightspeedZigZag>(card=> { LightspeedZigZag.CardInfo = card; });
            CustomCard.BuildCard<RandomZigZag>(card=> { RandomZigZag.CardInfo = card; });
            */

            foreach (CardInfo card in MyCursedCards)
            {
                if (card != new CardInfo() && card != null)
                {
                    ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);

                    card.enabled = false;
                }
            }


        }

        public static void CreateWorkingExplosion(Player player, Vector3 position, float maxRange = 5f, Gun ownerGun = null)
        {
            GameObject explosiveCard = Resources.Load<GameObject>("0 cards/Explosive bullet");
            if (explosiveCard == null) return;


            Gun gun = explosiveCard.GetComponent<Gun>();
            GameObject prefab = gun.objectsToSpawn[0].effect;
            if (prefab == null) return;

            GameObject explosionObj = GameObject.Instantiate(prefab, position, Quaternion.identity);
            explosionObj.name = "CustomExplosion";

            CircleCollider2D col = explosionObj.GetComponent<CircleCollider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }



            Explosion explosion = explosionObj.GetComponent<Explosion>();
            if (explosion == null) return;

            // Required for damage to happen:
            var spawnedAttack = explosionObj.AddComponent<SpawnedAttack>();
            spawnedAttack.spawner = player;
            explosionObj.AddComponent<PhotonView>(); // Optional for multiplayer

            if (ownerGun == null)
            ownerGun = player.data.weaponHandler.gun;

            // Optional: set damage values
            explosion.damage = 30f;
            explosion.range = 4f;

            explosion.hitPlayerAction = default;
            explosion.HitTargetAction = default;
            explosion.DealDamageAction = default;
            explosion.DealHealAction = default;


            float size = gun.damage/2f + 0.5f; 

            explosionObj.transform.localScale = new Vector3(size, size, size);


            Transform sparks = explosionObj.transform.GetChild(4);
            sparks.gameObject.SetActive(size >= 2f && size < 10f);
            sparks.rotation = new Quaternion(sparks.rotation.x, sparks.rotation.y + 90, sparks.rotation.z, sparks.rotation.w);

            sparks = explosionObj.transform.GetChild(5);
            sparks.gameObject.SetActive(size >= 10f);
            sparks.rotation = new Quaternion(sparks.rotation.x, sparks.rotation.y + 90, sparks.rotation.z, sparks.rotation.w);


            int rand = UnityEngine.Random.Range(0, 9);

            SoundImpactModifier mod = CardManager.cards.Values.Select(card => card.cardInfo).Where(card => card.cardName.ToLower() == "EXPLOSIVE BULLET".ToLower()).First().GetComponent<Gun>().soundImpactModifier;

            SoundManager.Instance.Play(mod.impactEnvironment, explosionObj.transform);

            GameObject.Destroy(explosionObj, 2f);
        }

        public static bool allowCard(Player player, CardInfo card)
        {
            bool allowed = true;
            if (ClassesRegistry.Get(card) != null)
            {
                allowed = ClassesRegistry.Get(card).PlayerIsAllowedCard(player);
            }

            return card.allowMultiple || !card.allowMultiple && !player.data.currentCards.Contains(card) && allowed;

        }
        public List<int> GetRoundWinners() => new List<int>(GameModeManager.CurrentHandler.GetRoundWinners());

        public void WinnerCardPower(List<int> winners)
        {
            foreach (WinnerCardEffect effect in FindObjectsOfType<WinnerCardEffect>())
            {
                if (effect != null)
                {
                    if (winners.Contains(effect.player.teamID))
                    {
                        CardInfo TheRandomCard = ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), Conditions.CommonCondition);
                        if (TheRandomCard == null)
                        {
                            // if there is no valid card, then try drawing from the list of all cards (inactive + active) but still make sure it is compatible
                            CardInfo[] allCards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList().Concat((List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToArray();
                            TheRandomCard = ModdingUtils.Utils.Cards.instance.DrawRandomCardWithCondition(allCards, effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), Conditions.CommonCondition);

                        }
                        if (TheRandomCard != null)
                        {
                            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(effect.player, TheRandomCard, addToCardBar: true);
                            ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(effect.player, TheRandomCard);
                        }
                    }
                }
            }
        }
        public void BoosterPackPower(List<int> winners)
        {
            foreach (BoosterPackEffect effect in FindObjectsOfType<BoosterPackEffect>())
            {
                if (effect != null && effect.player != null)
                {
                    if (winners.Contains(effect.player.teamID))
                    {

                        CardInfo TheRandomCard = null;

                        for (int i = 0; i < 2; i++)
                        {
                            TheRandomCard = ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), Conditions.AnyCondition);
                            if (TheRandomCard != null)
                            {
                                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(effect.player, TheRandomCard, addToCardBar: true);
                                ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(effect.player, TheRandomCard);
                            }
                        }
                        TheRandomCard = ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), Conditions.RareCondition);
                        if (TheRandomCard != null)
                        {
                            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(effect.player, TheRandomCard, addToCardBar: true);
                            ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(effect.player, TheRandomCard);
                        }
                    }
                }
            }
        }

        public void BoosterPackRemove()
        {
            foreach (BoosterPackEffect effect in FindObjectsOfType<BoosterPackEffect>())
            {
                while (effect.player.data.currentCards.Contains(effect.card))
                {
                    ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(effect.player, effect.card, ModdingUtils.Utils.Cards.SelectionType.Newest);
                }
            }
        }
        public int[] WinnerDraws = new int[50];
        public Player[] PlayersItsDonefor;
        private IEnumerator RoundEnd(IGameModeHandler gm)
        {
            PlayersItsDonefor = new Player[50];
            List<int> winners = GetRoundWinners();

            WinnerCardPower(winners);
            BoosterPackPower(winners);
            yield break;
        }
        private IEnumerator PickStart(IGameModeHandler gm)
        {
            AddCardToPlayerOnPickStart();
            BoosterPackRemove();
            yield break;
        }

        public void AddCardToPlayerOnPickStart()
        {
            foreach (AddCardPickStart effect in FindObjectsOfType<AddCardPickStart>())
            {
                if (effect != null && effect.player != null)
                {
                    System.Random rand = new System.Random();
                    if (rand.Next(100) < effect.chance || effect.chance == 0)
                    {
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(effect.player, effect.card, addToCardBar: true);
                        CardBarUtils.instance.ShowAtEndOfPhase(effect.player, effect.card);
                    }
                }
            }
        }


        public IEnumerator DoubleCardPickEnd(IGameModeHandler gm)
        {
            foreach (DualPickEffect effect in FindObjectsOfType<DualPickEffect>())
            {
                CardInfo card = effect.player.data.currentCards.Last();
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(effect.player, card, addToCardBar: true);
                CardBarUtils.instance.ShowCard(effect.player, card);
            }

            GameModeManager.AddOnceHook(GameModeHooks.HookRoundStart, LarrysCards.instance.RoundStartDoubleCardPick);
            yield break;
        }

        public IEnumerator PrepareDoubleCardPick(IGameModeHandler gm)
        {
            GameModeManager.AddOnceHook(GameModeHooks.HookPickEnd, LarrysCards.instance.DoubleCardPickEnd);
            yield break;
        }

        public IEnumerator RoundStartDoubleCardPick(IGameModeHandler gm)
        {
            foreach (DualPickEffect effect in FindObjectsOfType<DualPickEffect>())
            {
                CardBarUtils.instance.HideCard(effect.player);

                Destroy(effect);
            }
                yield break;
        }



        public static Vector2 RotatedBy(Vector2 spinningpoint, float degrees, Vector2 center = default)
        {
            double radians = (float)((double)degrees * (Math.PI / 180.0));
            float num = (float)Math.Cos(radians);
            float num2 = (float)Math.Sin(radians);
            Vector2 vector = spinningpoint - center;
            Vector2 result = center;
            result.x += vector.x * num - vector.y * num2;
            result.y += vector.x * num2 + vector.y * num;
            return result;
        }

        private IEnumerator ResetCardChoiceStuff(IGameModeHandler gm)
        {
            foreach (Player target in PlayerManager.instance.players)
            {
                LarrysCards_CardChoicesPatch.ClearForcedChoices(target);
            }
            yield break;
        }

        private IEnumerator ResetCardChoiceOverrideSlots(IGameModeHandler gm)
        {
            LarrysCards_CardExtraInfoPatch.DestroyObject();

            foreach (Player target in PlayerManager.instance.players)
            {
                LarrysCards_CardChoicesPatch.ClearOverridedSlots(target);
            }
            yield break;
        }
    }
    public class ProjectileAdder : MonoBehaviour
    {
        public ProjectilesToSpawn[] projectiles;

        public void Start()
        {
            Gun gun = GetComponentInParent<WeaponHandler>().gun;
            List<ProjectilesToSpawn> gunProjectiles = gun.projectiles.ToList();
            gunProjectiles.AddRange(projectiles);
            gun.projectiles = gunProjectiles.ToArray();
        }
        public void OnDestroy()
        {

            Gun gun = GetComponentInParent<WeaponHandler>().gun;
            List<ProjectilesToSpawn> gunProjectiles = gun.projectiles.ToList();
            foreach (var projectile in projectiles)
                gunProjectiles.Remove(projectile);
            gun.projectiles = gunProjectiles.ToArray();
        }
    }

    public class AddCardPickStart : MonoBehaviour
    {
        public Player player;
        public CardInfo card;
        public int chance = 0;
        public void Start()
        {
            player = gameObject.GetComponentInParent<Player>();
        }
    }
}