using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using BreadCards.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;

namespace BreadCards
{
    // These are the mods required for our mod to work
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    // Declares our mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our mod is associated with
    [BepInProcess("Rounds.exe")]
    public class BreadCards : BaseUnityPlugin
    {
        private const string ModId = "com.willuwontu.rounds.BreadCards";
        private const string ModName = "Bread's Cards";
        public const string Version = "1.0.0"; // What version are we on (major.minor.patch)?
        public const string ModInitials = "Bread's Cards";

        public static BreadCards instance { get; private set; }

        void Awake()
        {
            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start()
        {
            instance = this;
            CustomCard.BuildCard<DoubleShot>();
            CustomCard.BuildCard<BlockingMaster>();
            CustomCard.BuildCard<HopeIDontMiss>();
            CustomCard.BuildCard<AmmoSavingTechnology>();
            CustomCard.BuildCard<SprayAndPray>();
        }
    }
}