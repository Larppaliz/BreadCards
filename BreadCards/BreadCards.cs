using BepInEx;
using BreadCards.Cards.BulletMods;
using BreadCards.Cards.Classes.Magnet;
using BreadCards.Cards.Classes.Shulker;
using BreadCards.Cards.Debuff;
using BreadCards.Cards.General;
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

namespace BreadCards
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
    public class BreadCards : BaseUnityPlugin
    {
        public static BreadCards instance { get; private set; }

        private const string ModId = "larppaliz.rounds.breadcards";
        private const string ModName = "Bread's Cards Remake";
        public const string Version = "0.1.0"; // What version are we on (major.minor.patch)?
        public const string ModInitials = "BCrm";

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
            CustomCard.BuildCard<ClusterShots>(c => { ClusterShots.CardInfo = c; });

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
            CustomCard.BuildCard<RiceMarketEffect>(c =>  RiceMarketEffect.CardInfo = c );
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
        public static bool CommonCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            bool allowed = true;
            if (ClassesRegistry.Get(card) != null)
            {
                allowed = ClassesRegistry.Get(card).PlayerIsAllowedCard(player);
            }
            return (card.rarity == CardInfo.Rarity.Common || card.rarity == CardInfo.Rarity.Uncommon) && allowed;

        }

        public static bool RareCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            bool allowed = true;
            if (ClassesRegistry.Get(card) != null)
            {
                allowed = ClassesRegistry.Get(card).PlayerIsAllowedCard(player);
            }
            return card.rarity == CardInfo.Rarity.Rare && allowed;
        }

        public static bool AnyCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            bool allowed = true;
            if (ClassesRegistry.Get(card) != null)
            {
                allowed = ClassesRegistry.Get(card).PlayerIsAllowedCard(player);
            }
            return (!MyCursedCards.Contains(card)) && allowed;
        }

        public void WinnerCardPower(List<int> winners)
        {
            foreach (WinnerCardEffect effect in FindObjectsOfType<WinnerCardEffect>())
            {
                if (effect != null)
                {
                    if (winners.Contains(effect.player.teamID))
                    {
                        CardInfo TheRandomCard = ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), CommonCondition);
                        if (TheRandomCard == null)
                        {
                            // if there is no valid card, then try drawing from the list of all cards (inactive + active) but still make sure it is compatible
                            CardInfo[] allCards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList().Concat((List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToArray();
                            TheRandomCard = ModdingUtils.Utils.Cards.instance.DrawRandomCardWithCondition(allCards, effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), CommonCondition);

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
                            TheRandomCard = ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), AnyCondition);
                            if (TheRandomCard != null)
                            {
                                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(effect.player, TheRandomCard, addToCardBar: true);
                                ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(effect.player, TheRandomCard);
                            }
                        }
                        TheRandomCard = ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(effect.player, effect.player.data.weaponHandler.gun, effect.player.data.weaponHandler.gun.GetComponent<GunAmmo>(), effect.player.data, effect.player.data.healthHandler, effect.player.GetComponent<Gravity>(), effect.player.data.block, effect.player.data.GetComponent<CharacterStatModifiers>(), RareCondition);
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

            if (!gun)
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

        private IEnumerator ResetCardChoiceStuff(IGameModeHandler gm)
        {
            foreach (Player target in PlayerManager.instance.players)
            {
                BreadCards_CardChoicesPatch.ClearForcedChoices(target);
            }
            yield break;
        }

        private IEnumerator ResetCardChoiceOverrideSlots(IGameModeHandler gm)
        {
            BreadCards_CardExtraInfoPatch.DestroyObject();

            foreach (Player target in PlayerManager.instance.players)
            {
                BreadCards_CardChoicesPatch.ClearOverridedSlots(target);
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

    [HarmonyPatch(typeof(CardChoice), "Update")]
    public static class BreadCards_CardExtraInfoPatch
    {
        public static Dictionary<string, Func<Player, CardInfo>> extraInfoCardData = new Dictionary<string, Func<Player, CardInfo>>();

        public static GameObject shownObject;
        public static int selectedCard = -1;
        public static int spawnedCount = 0;

        public static List<CardInfo> getCardsFromGameObjects(List<GameObject> gameObjects)
        {
            List<CardInfo> cards = new List<CardInfo>();

            foreach (GameObject obj in gameObjects)
            {
                if (obj.GetComponent<CardInfo>())
                {
                    CardInfo info = obj.GetComponent<CardInfo>();
                    cards.Add(info);
                }
            }

            return cards;
        }

        public static void DestroyObject()
        {
            selectedCard = -1;

            if (shownObject != null) GameObject.Destroy(shownObject);
            shownObject = null;
        }

        public static void Postfix(ref CardChoice __instance, ref int ___currentlySelectedCard, ref List<GameObject> ___spawnedCards, ref Transform[] ___children)
        {
            if (CardChoiceVisuals.instance != null)
            {

                if (__instance.IsPicking)
                {

                    int index = ___currentlySelectedCard;


                    int count = ___spawnedCards.Count;

                    if (spawnedCount != count) { DestroyObject(); }

                    spawnedCount = count;

                    if (index != selectedCard)
                    {
                        DestroyObject();
                    }

                    selectedCard = index;

                    if (shownObject == null)
                    {
                        List<CardInfo> cards = getCardsFromGameObjects(___spawnedCards);

                        Player player = PlayerManager.instance.players[__instance.pickrID];

                        if (player == null) return;

                        CardInfo card = null;

                        if (cards.Count - 1 < index || index < 0) return;

                        if (extraInfoCardData.ContainsKey(cards[index].cardName)) card = extraInfoCardData[cards[index].cardName].Invoke(player);

                        if (card == null)
                        {
                            return;
                        }

                        Transform cardTransform = ___spawnedCards[index].transform;


                        Vector3 baseOffset = cardTransform.right;

                        if (cardTransform.position.x > 0) baseOffset *= -1f;

                        if (cardTransform.rotation.eulerAngles.z == 0) baseOffset = cardTransform.up * -1.3f;

                        shownObject = __instance.AddCardVisual(card, new Vector3(0, 0, 0));
                        Vector3 rightOffset = baseOffset * 12f;
                        rightOffset.x *= cardTransform.localScale.x;
                        rightOffset.y *= cardTransform.localScale.y;
                        rightOffset.z *= cardTransform.localScale.z;

                        shownObject.transform.localScale = cardTransform.localScale * 0.8f;
                        shownObject.transform.position = cardTransform.position + rightOffset;
                        shownObject.transform.AddZPosition(-15);
                        shownObject.transform.rotation = cardTransform.rotation;

                    }
                }
                else DestroyObject();
            }
        }
    }

    [HarmonyPatch(typeof(CardChoice), "SpawnUniqueCard")]
    public static class BreadCards_CardChoicesPatch
    {
        public static Dictionary<int, List<ForcedCardRequest>> pendingForcedCards = new Dictionary<int, List<ForcedCardRequest>>();

        public static Dictionary<int, List<ForcedCardRequest>> readyForcedCards = new Dictionary<int, List<ForcedCardRequest>>();

        public static void AddForcedCardChoice(Player player, ForcedCardRequest fcr)
        {
            if (!pendingForcedCards.ContainsKey(player.playerID))
                pendingForcedCards.Add(player.playerID, new List<ForcedCardRequest>());

            pendingForcedCards[player.playerID].Add(fcr);
        }

        public static bool RemoveForcedCardChoice(Player player, ForcedCardRequest fcr)
        {
            if (!pendingForcedCards.ContainsKey(player.playerID)) return false;

            return pendingForcedCards[player.playerID].Remove(fcr);
        }
        public static void RemoveFromReadyForcedCards(int playerID, ForcedCardRequest fcr)
        {
            if (!readyForcedCards.ContainsKey(playerID))
                return;

            if (!readyForcedCards[playerID].Contains(fcr))
                return;

            readyForcedCards[playerID].Remove(fcr);
        }

        public static void ClearForcedChoices(Player player)
        {
            int playerId = PlayerManager.instance.players.IndexOf(player);

            if (pendingForcedCards == null) pendingForcedCards = new Dictionary<int, List<ForcedCardRequest>>();

            if (pendingForcedCards.ContainsKey(playerId))
            {
                pendingForcedCards.Remove(playerId);
            }
        }

        public static void ClearOverridedSlots(Player player)
        {
            int playerId = PlayerManager.instance.players.IndexOf(player);

            if (overridedSlotsThisRun == null) overridedSlotsThisRun = new Dictionary<int, List<int>>();

            if (overridedSlotsThisRun.ContainsKey(playerId))
            {
                overridedSlotsThisRun[playerId] = new List<int>();
            }
        }

        static Dictionary<int, List<int>> overridedSlotsThisRun = new Dictionary<int, List<int>>();

        public static void SetBetterSlots(Player player, int maxCount)
        {
            int[] slotCounts = new int[maxCount];

            foreach (ForcedCardRequest card in readyForcedCards[player.playerID])
            {
                if (card.reverse) card.slot = maxCount - card.slot - 1;

                if (card.slot == -1)
                {
                    int tempSlot = 0;

                    for (int i = 0; i < 100; i++)
                    {
                        if (slotCounts[tempSlot] > 0)
                        {
                            tempSlot = UnityEngine.Random.Range(0, maxCount);
                        }
                    }

                    if (slotCounts[tempSlot] == 0)
                    {
                        card.slot = tempSlot;
                    }
                    else return;

                }
                else for (int i = 0; i < maxCount; i++)
                    {
                        if (slotCounts[card.slot] > 0)
                        {
                            if (card.reverse) card.slot--;
                            else card.slot++;
                        }
                        else break;
                    }

                if (card.slot >= 0 && card.slot < maxCount) slotCounts[card.slot]++;
            }

        }

        

        [HarmonyPriority(0)]
        [HarmonyAfter(new string[] { "com.Root.Null", "com.willuwontu.rounds.cards", "pykess.rounds.plugins.cardchoicespawnuniquecardpatch", "pykess.rounds.plugins.pickphaseshenanigans" })]
        public static void Postfix(ref CardChoice __instance, ref GameObject __result, ref List<GameObject> ___spawnedCards, ref Transform[] ___children, ref int ___pickrID)
        {

            if (___pickrID < 0 || ___pickrID > PlayerManager.instance.players.Count || PlayerManager.instance == null || PlayerManager.instance.players == null) return;

            Player player = PlayerManager.instance.players[___pickrID];

            if (player == null || pendingForcedCards == null || __instance == null || __result == null || ___spawnedCards == null || ___children == null) return;

            if (!pendingForcedCards.ContainsKey(___pickrID)) return;

            if (overridedSlotsThisRun == null) overridedSlotsThisRun = new Dictionary<int, List<int>>();

            if (___spawnedCards.Count == 0)
            {
                readyForcedCards = pendingForcedCards.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value
                        .Where(fcr => fcr.condition(player))
                        .Select(fcr => fcr.Clone())
                        .ToList());

                SetBetterSlots(player, ___children.Count());
            }

            for (int i = 0; i < readyForcedCards[___pickrID].Count; i++)
            {
                ForcedCardRequest fcr = readyForcedCards[___pickrID][i];

                int slot = fcr.slot;


                if (___spawnedCards.Count == slot)
                {
                    CardInfo card = null;

                    if (fcr.customRoll == null) card = fcr.card;

                    if (card == null && fcr.customRoll != null)
                    {
                        card = fcr.customRoll?.Invoke(player);
                    }

                    if (card == null) { card = Rice.CardInfo; BreadCards.print("Card was null, turned it into RICE"); }

                    if (!overridedSlotsThisRun.ContainsKey(player.playerID)) overridedSlotsThisRun.Add(player.playerID, new List<int>());



                    overridedSlotsThisRun[player.playerID].AddItem(slot);

                    RemoveFromReadyForcedCards(player.playerID, fcr);


                    GameObject old = __result;
                    if (BreadCards.instance == null || card.gameObject == null) return;

                    BreadCards.instance.ExecuteAfterFrames(3, delegate
                    {
                        PhotonNetwork.Destroy(old);
                    });
                    __result = (GameObject)typeof(CardChoice).GetMethod("Spawn", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[3]
                    {
                                card.gameObject,
                                __result.transform.position,
                                __result.transform.rotation
                    });
                    __result.GetComponent<CardInfo>().sourceCard = card;
                    __result.GetComponentInChildren<DamagableEvent>().GetComponent<Collider2D>().enabled = false;

                    break;
                }
            }
        }
    }

    public class ForcedCardRequest
    {
        public ForcedCardRequest Clone()
        {
            return new ForcedCardRequest
            {
                card = this.card, // deep clone if needed
                slot = this.slot,
                fill = this.fill,
                reverse = this.reverse,
                customRoll = this.customRoll,
                condition = this.condition
            };
        }

        // condition for the card to be added to the draw that is checked each draw (see DoubleIt.cs)
        public Func<Player, bool> condition = (_) => true;

        // for special things like rolling a random card eachtime (see DarkWebDeals.cs or UltimateCopyCat.cs)
        public Func<Player, CardInfo> customRoll;

        // use if set card (see RiceMarketEffect.cs)
        public CardInfo card;

        // the slot the card is replacing
        public int slot;

        // ex: whetever or not the card will move from slot 1 to 2 if slot 1 was already overrided
        public bool fill;

        // reverses the slots so 0 is now the last card and 1 is the second last
        public bool reverse;
    }
}