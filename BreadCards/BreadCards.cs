using BepInEx;
using BepInEx.Configuration;
using BreadCards.Cards;
using BreadCards.Cards.Classes.Shulker;
using BreadCards.Cards.Classes.ZigZag;
using BreadCards.Cards.Debuff;
using HarmonyLib;
using ModdingUtils.Utils;
using ModsPlus;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TMPro;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnboundLib.Utils;
using UnboundLib.Utils.UI;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

namespace BreadCards
{
    // These are the mods required for our mod to work
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.classes.manager.reborn", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.CrazyCoders.Rounds.RarityBundle", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willis.rounds.modsplus", BepInDependency.DependencyFlags.HardDependency)]
    // Declares our mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our mod is associated with
    [BepInProcess("Rounds.exe")]
    public class BreadCards : BaseUnityPlugin
    {
        public static BreadCards instance { get; private set; }

        private const string ModId = "BreadCards";
        private const string ModName = "Bread's Cards";
        public const string Version = "3.1.0"; // What version are we on (major.minor.patch)?
        public const string ModInitials = "Breads Cards";

        public static ConfigEntry<bool> BBConfig;

        public static bool BlockBalance = false;

        public static ConfigEntry<bool> EnableWinnerDrawLessConfig;
        public static ConfigEntry<int> WinnerDrawAmountConfig;

        public static ConfigEntry<int> StartingPicksConfig;

        public static int StartingPicks = 1;

        public static ConfigEntry<int> StartingDrawsConfig;

        public static int StartingDraws = 0;

        public static bool enableWinnerDrawLess = false;
        public static int winnerDrawAmount = 2;


        public static ConfigEntry<float> MinBlockCooldown;

        public static float BCDm = 0f;

        public GameObject optionsMenu;

        void Awake()
        {
            instance = this;

            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
            GameModeManager.AddHook(GameModeHooks.HookPickEnd, PickEnd);
            GameModeManager.AddHook(GameModeHooks.HookPickStart, PickStart);
            GameModeManager.AddHook(GameModeHooks.HookRoundEnd, RoundEnd);
            GameModeManager.AddHook(GameModeHooks.HookGameStart, GameStart);

            {
                EnableWinnerDrawLessConfig = Config.Bind(ModInitials, "WinnerDrawToggle", false, "Toggle the winner draw stuff.");
                WinnerDrawAmountConfig = Config.Bind(ModInitials, "WinnerDrawAmount", 2, "Winner Draw Amount");
                StartingPicksConfig = Config.Bind(ModInitials, "StartingPicksAmount", 1, "Starting Picks Amount");
                StartingDrawsConfig = Config.Bind(ModInitials, "StartingDrawsAmount", 0, "Extra Starting Draws Amount");
                //MinBlockCooldown = Config.Bind(ModInitials, "MinBlockCD", 0f, "Toggle the winner draw stuff.");
                BBConfig = Config.Bind(ModInitials, "BlockBalance", false, "Toggle Block Balance");
            }
        }
        public static List<CardInfo> MyCursedCards = new List<CardInfo>();
        void Start()
        {
            Unbound.RegisterHandshake(ModId, OnHandShakeCompleted);
            Unbound.RegisterMenu("Breads Cards", () => { }, BreadGUI, optionsMenu, false);

            StartingPicks = StartingPicksConfig.Value;
            StartingDraws = StartingDrawsConfig.Value;
            enableWinnerDrawLess = EnableWinnerDrawLessConfig.Value;
            winnerDrawAmount = WinnerDrawAmountConfig.Value;


            CustomCard.BuildCard<DoubleShot>();
            CustomCard.BuildCard<BlockingMaster>();
            CustomCard.BuildCard<Rebuild>();
            CustomCard.BuildCard<FocusedShots>();
            CustomCard.BuildCard<SprayAndPray>();
            CustomCard.BuildCard<SuperBurst>();
            CustomCard.BuildCard<BulletStream>();
            CustomCard.BuildCard<SuperShotgun>();
            CustomCard.BuildCard<QuickswitchMagazine>();
            CustomCard.BuildCard<NoBounces>();
            CustomCard.BuildCard<TimelessBullets>();
            CustomCard.BuildCard<DualBurst>();

            CustomCard.BuildCard<IWantOneToo>();
            CustomCard.BuildCard<CardDealer>();
            CustomCard.BuildCard<DoubleIt>();
            CustomCard.BuildCard<StaticBullets>();

            CustomCard.BuildCard<FuseBullets>();
            CustomCard.BuildCard<BouncesToBullets>();
            CustomCard.BuildCard<ExtraDamage>();
            CustomCard.BuildCard<LightSpeedBuckshot>();
            CustomCard.BuildCard<BarrelMod>();
            CustomCard.BuildCard<MoonBullets>();

            CustomCard.BuildCard<AccelleratingShots>(c => { AccelleratingShots.CardInfo = c; });
            CustomCard.BuildCard<GravityInverterShots>(c => { GravityInverterShots.CardInfo = c; });
            CustomCard.BuildCard<BadHoming>(c => { BadHoming.CardInfo = c; });

            CustomCard.BuildCard<SplitShot>(c => { SplitShot.CardInfo = c; });
            CustomCard.BuildCard<ProximityClusterBomb>(c => { ProximityClusterBomb.CardInfo = c; });

            CustomCard.BuildCard<HoppingBullets>(c => { HoppingBullets.CardInfo = c; });
            CustomCard.BuildCard<PlayerControlledBullets>(c => { PlayerControlledBullets.CardInfo = c; });
            CustomCard.BuildCard<StalkerBullets>(c => { StalkerBullets.CardInfo = c; });
            CustomCard.BuildCard<LagBullets>(c => { LagBullets.CardInfo = c; });
            CustomCard.BuildCard<BugBullets>(c => { BugBullets.CardInfo = c; });


            CustomCard.BuildCard<BlockHoming>(c => { BlockHoming.CardInfo = c; });
            CustomCard.BuildCard<BlockStop>(c => { BlockStop.CardInfo = c; });
            CustomCard.BuildCard<BlockReverse>(c => { BlockReverse.CardInfo = c; });

            // CustomCard.BuildCard<ProximityFuse>((card) => { ProximityFuse.CardInfo = card; });

            CustomCard.BuildCard<CardTricks>();
            CustomCard.BuildCard<Copyer>();

            //CustomCard.BuildCard<CardUpgrade>();

            CustomCard.BuildCard<UltraDefense>();
            CustomCard.BuildCard<MultiplyingSlime>();

            CustomCard.BuildCard<BlackMarket>();
            CustomCard.BuildCard<RiceMarket>();
            //CustomCard.BuildCard<BoosterPack>();


            CustomCard.BuildCard<RiceMarketEffect>(c => { RiceMarketEffect.CardInfo = c; });
            CustomCard.BuildCard<Rice>(c => { Rice.CardInfo = c; });

            CustomCard.BuildCard<BlackMarketCopy>(c => { BlackMarketCopy.CardInfo = c; });

            CustomCard.BuildCard<UltraDefenseCopy>(c => { UltraDefenseCopy.CardInfo = c; });
            CustomCard.BuildCard<MultipliedSlime>(c => { MultipliedSlime.CardInfo = c; });

            //Debuff givers
            CustomCard.BuildCard<TrueEvil>();
            CustomCard.BuildCard<CurseOfTheTroopers>();
            CustomCard.BuildCard<BigMonkeAttack>();
            CustomCard.BuildCard<BouncySolution>();
            CustomCard.BuildCard<StickySolution>();

            //Debuff Cards
            CustomCard.BuildCard<EvilCurse>(card => { EvilCurse.CardInfo = card; });
            CustomCard.BuildCard<BigMonkeCurse>(card => { BigMonkeCurse.CardInfo = card; });
            CustomCard.BuildCard<TrooperCurse>(card => { TrooperCurse.CardInfo = card; });
            CustomCard.BuildCard<Bounce>(card => { Bounce.CardInfo = card; });
            CustomCard.BuildCard<Sticky>(card => { Sticky.CardInfo = card; });


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

            /* /Zig zag Class
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
        public List<int> GetRoundWinners() => new List<int>(GameModeManager.CurrentHandler.GetRoundWinners());
        internal void OnHandShakeCompleted()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < 1; i++)
                {
                    NetworkingManager.RPC_Others(typeof(BreadCards), nameof(UpdateValues), enableWinnerDrawLess, winnerDrawAmount, BlockBalance, BCDm, StartingPicks, StartingDraws);
                }
            }
        }
        private static void UpdateValues(bool WinnerDraw, int WinnerDrawValue, bool BB,  float BCD, int Picks, int Draws)
        {
            StartingPicks = Picks;
            StartingDraws = Draws;
            enableWinnerDrawLess = WinnerDraw;
            winnerDrawAmount = WinnerDrawValue;
            BlockBalance = BB;
            BCDm = BCD;
        }
        private void BreadGUI(GameObject menu)
        {
            if (menu == null)
            {
                Debug.LogError("Menu object is null.");
                return;
            }
            MenuHandler.CreateText("Drawing Cards", menu, out TextMeshProUGUI _);
            MenuHandler.CreateToggle(EnableWinnerDrawLessConfig.Value, "Toggle Winner Draws", menu, value => { EnableWinnerDrawLessConfig.Value = value; enableWinnerDrawLess = value; });
            MenuHandler.CreateSlider("How many cards the winner gets to draw", menu, 20, 1, 20, winnerDrawAmount, value => { WinnerDrawAmountConfig.Value = (int)value; winnerDrawAmount = (int)value; }, out UnityEngine.UI.Slider WinnerAmountSlider, true);
            MenuHandler.CreateSlider("How many cards you pick at the start", menu, 20, 1, 20, StartingPicks, value => { StartingPicksConfig.Value = (int)value; StartingPicks = (int)value; }, out UnityEngine.UI.Slider StartPickAmountSlider, true);
            MenuHandler.CreateSlider("How many extra cards you draw at the start", menu, 20, -5, 20, StartingDraws, value => { StartingDrawsConfig.Value = (int)value; StartingDraws = (int)value; }, out UnityEngine.UI.Slider StartDrawAmountSlider, true);
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, 30);
            MenuHandler.CreateText("Rest hidden because they dont work", menu, out TextMeshProUGUI _);
                MenuHandler.CreateText("Stats", menu, out TextMeshProUGUI _);
                MenuHandler.CreateSlider("Minimum Block CD", menu, 20, 0f, 5f, BCDm, value => { MinBlockCooldown.Value = value; BCDm = value; }, out UnityEngine.UI.Slider cooldownSlider, false, new Color(120, 150, 255));
                MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, 30);
                MenuHandler.CreateText("Misc", menu, out TextMeshProUGUI _);
                MenuHandler.CreateToggle(BBConfig.Value, "Toggle block cooldown balancing", menu, value => { BBConfig.Value = value; BlockBalance = value; });
              
        }

        public bool CommonCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            return (card.rarity == CardInfo.Rarity.Common || card.rarity == CardInfo.Rarity.Uncommon);

        }

        public bool RareCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            return (card.rarity == CardInfo.Rarity.Rare);
        }

        public bool AnyCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            // do not allow duplicates of cards with allowMultiple == false (handled by moddingutils)
            // card rarity must be as desired
            // card cannot be another cardmanipulation card
            // card cannot be from a blacklisted catagory of any other card (handled by moddingutils)

            return (!MyCursedCards.Contains(card));
        }

        public void WinnerCardPower(List<int> winners)
        {
            foreach (WinnerCardEffect effect in FindObjectsOfType<WinnerCardEffect>())
            {
                if (effect != null)
                {
                    if (winners.Contains(effect.player.teamID))
                    {
                        CardInfo TheRandomCard = ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), this.CommonCondition);
                        if (TheRandomCard == null)
                        {
                            // if there is no valid card, then try drawing from the list of all cards (inactive + active) but still make sure it is compatible
                            CardInfo[] allCards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList().Concat((List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToArray();
                            TheRandomCard = ModdingUtils.Utils.Cards.instance.DrawRandomCardWithCondition(allCards, effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), this.CommonCondition);

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
                            TheRandomCard = ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), this.AnyCondition);
                            if (TheRandomCard != null)
                            {
                                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(effect.player, TheRandomCard, addToCardBar: true);
                                ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(effect.player, TheRandomCard);
                            }
                        }
                        TheRandomCard = ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), this.RareCondition);
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
            if (EnableWinnerDrawLessConfig.Value && winners != null && DrawNCards.DrawNCards.NumDrawsConfig != null)
            {
                foreach (Player player in PlayerManager.instance.players)
                {
                    if (winners.Contains(player.teamID))
                    {
                        WinnerDraws[player.playerID] = DrawNCards.DrawNCards.GetPickerDraws(player.playerID);
                        DrawNCards.DrawNCards.RPCA_SetPickerDraws(player.playerID, winnerDrawAmount + (DrawNCards.DrawNCards.GetPickerDraws(player.playerID) - DrawNCards.DrawNCards.NumDraws));
                    }
                }
            }
            WinnerCardPower(winners);
            BoosterPackPower(winners);
            yield break;
        }

        public void fixWinnerDrawThing(Player player)
        {
            if (EnableWinnerDrawLessConfig.Value && !PlayersItsDonefor.Contains(player))
            {
                List<int> winners = GetRoundWinners();
                if (winners != null)
                {
                    if (winners.Contains(player.teamID) && WinnerDraws[player.playerID] != DrawNCards.DrawNCards.GetPickerDraws(player.playerID))
                    {
                        PlayersItsDonefor[player.playerID] = player;
                        DrawNCards.DrawNCards.RPCA_SetPickerDraws(player.playerID, WinnerDraws[player.playerID]);
                    }
                }
            }
        }
        private IEnumerator PickEnd(IGameModeHandler gm)
        {
            SelectAnyNumberRounds.Plugin.configPickNumber.Value = 1;

            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                if (PlayersItsDonefor != null)
                {
                    if (PlayerManager.instance.players != null && PlayersItsDonefor.Count() > 1)
                    {
                        fixWinnerDrawThing(PlayerManager.instance.players[i]);
                    }
                }
            }
            yield break;
        }

        private IEnumerator PickStart(IGameModeHandler gm)
        {
            AddCardToPlayerOnPickStart();
            BoosterPackRemove();
            yield break;
        }

        private IEnumerator GameEnd(IGameModeHandler gm)
        {
            SelectAnyNumberRounds.Plugin.configPickNumber.Value = 2;
            yield break;
        }

        private IEnumerator FirstRoundStart(IGameModeHandler gm)
        {
            foreach (Player player in PlayerManager.instance.players)
            {
                DrawNCards.DrawNCards.RPCA_SetPickerDraws(player.playerID, DrawNCards.DrawNCards.GetPickerDraws(player.playerID) - StartingDraws);
            }
            yield break;
        }
        private IEnumerator GameStart(IGameModeHandler gm)
        {
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                Player player = PlayerManager.instance.players[i];

                player.playerID = i;

            }

            SelectAnyNumberRounds.Plugin.enableContinueCard.Value = false;
            SelectAnyNumberRounds.Plugin.configPickNumber.Value = StartingPicks;

            foreach (Player player in PlayerManager.instance.players)
            {
                DrawNCards.DrawNCards.RPCA_SetPickerDraws(player.playerID, DrawNCards.DrawNCards.NumDraws + StartingDraws);

                player.gameObject.GetOrAddComponent<BlockBalancer>();
            }



            GameModeManager.AddOnceHook(GameModeHooks.HookRoundStart, FirstRoundStart);

            yield break;
        }

        public int PlayerDrawsIncrease(Player player, int Amount)
        {
            if (GameModeManager.CurrentHandler.GetTeamScore(player.teamID).rounds > 0)
            {
                BreadCards.instance.fixWinnerDrawThing(player);
            }

            DrawNCards.DrawNCards.RPCA_SetPickerDraws(player.playerID, DrawNCards.DrawNCards.GetPickerDraws(player.playerID) + Amount);

            return DrawNCards.DrawNCards.GetPickerDraws(player.playerID);
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

            GameModeManager.AddOnceHook(GameModeHooks.HookRoundStart, BreadCards.instance.RoundStartDoubleCardPick);
            yield break;
        }

        public IEnumerator PrepareDoubleCardPick(IGameModeHandler gm)
        {
            GameModeManager.AddOnceHook(GameModeHooks.HookPickEnd, BreadCards.instance.DoubleCardPickEnd);
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



        public static Vector2 RotatedBy(Vector2 spinningpoint, float degrees, Vector2 center = default(Vector2))
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

        public static float Distance(Vector2 value1, Vector2 value2)
        {
            float num = value1.x - value2.x;
            float num2 = value1.y - value2.y;
            return (float)Math.Sqrt(num * num + num2 * num2);
        }

        public static Vector2 DirectionTo(Vector2 Origin, Vector2 Target)
        {
            return Normalize(Target - Origin);
        }

        public static Vector2 Normalize(Vector2 value)
        {
            float num = 1f / (float)Math.Sqrt(value.x * value.x + value.y* value.y);
            value.x *= num;
            value.y *= num;
            return value;
        }


        public static (GameObject AddToProjectile, GameObject effect, Explosion explosion) LoadExplosion(string name, Gun? gun = null)
        {
            // load explosion effect from Explosive Bullet card
            GameObject? explosiveBullet = (GameObject)Resources.Load("0 cards/Explosive bullet");

            Gun explosiveGun = explosiveBullet.GetComponent<Gun>();

            if (gun != null)
            {
                // change the gun sounds
                gun.soundGun.AddSoundImpactModifier(explosiveGun.soundImpactModifier);
            }

            // load assets
            GameObject A_ExplosionSpark = explosiveGun.objectsToSpawn[0].AddToProjectile;
            GameObject explosionCustom = Instantiate(explosiveGun.objectsToSpawn[0].effect);
            explosionCustom.transform.position = new Vector3(1000, 0, 0);
            explosionCustom.hideFlags = HideFlags.HideAndDontSave;
            explosionCustom.name = name;
            DestroyImmediate(explosionCustom.GetComponent<RemoveAfterSeconds>());
            Explosion explosion = explosionCustom.GetComponent<Explosion>();

            return (A_ExplosionSpark, explosionCustom, explosion);
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

    public class BlockBalancer : MonoBehaviour
    {
        public Player player;
        public void Start()
        {
            player = gameObject.GetComponentInParent<Player>();
        }

        public void Update()
        {

            if (player.data.block.IsBlocking())
            {
                player.data.block.counter *= 0f;
            }

            if (player.data.block.Cooldown() < 1f)
            {
                player.data.block.cdAdd += 0.1f;
            }
        }
    }

    [HarmonyPatch(typeof(ProjectileCollision))]
    public class ProjectileCollisionPatch
    {
        [HarmonyPatch("HitSurface")]
        [HarmonyPrefix]
        public static bool hitSurface(ProjectileCollision __instance, ref ProjectileHitSurface.HasToStop __result, GameObject projectile, HitInfo hit)
        {
                if (projectile.GetComponent<ProjectileHit>().ownPlayer == __instance.GetComponentInParent<ProjectileHit>().ownPlayer)
                {
                    return false;
                }

                return true;
        }
    }
}