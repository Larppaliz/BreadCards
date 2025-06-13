using UnityEngine;

namespace LarrysCards
{
    internal static class Assets
    {

        private static readonly AssetBundle Bundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("coolroundsartlol", typeof(LarrysCards).Assembly);


        public static GameObject BlackMarketArt = Bundle.LoadAsset<GameObject>("C_BlackMarket");
        public static GameObject BadBlackMarketArt = Bundle.LoadAsset<GameObject>("C_BadBlackMarket");
        public static GameObject CardCopierArt = Bundle.LoadAsset<GameObject>("C_CardCopier");
        public static GameObject OldCardCopierArt = Bundle.LoadAsset<GameObject>("C_OldCardCopier");
        public static GameObject AnvilArt = Bundle.LoadAsset<GameObject>("C_Anvil");
        public static GameObject ShulkerArt = Bundle.LoadAsset<GameObject>("C_Shulker");
        public static GameObject StalkerArt = Bundle.LoadAsset<GameObject>("C_StalkerBullets");
        public static GameObject ShulkerShotsArt = Bundle.LoadAsset<GameObject>("C_ShulkerShots");
        public static GameObject ActivatorArt = Bundle.LoadAsset<GameObject>("C_Activator");

        public static GameObject MagnetArt = Bundle.LoadAsset<GameObject>("C_Magnet");
        public static GameObject MagnetShotsArt = Bundle.LoadAsset<GameObject>("C_MagnetShots");

        public static GameObject UltraDefenseArt = Bundle.LoadAsset<GameObject>("C_UltraDefense");
        public static GameObject UltraDefendedArt = Bundle.LoadAsset<GameObject>("C_UltraDefended");
        public static GameObject ZigZagArt = Bundle.LoadAsset<GameObject>("C_ZigZag");
    }
}
