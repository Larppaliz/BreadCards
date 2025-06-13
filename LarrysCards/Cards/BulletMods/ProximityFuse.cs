/*
namespace BreadCards.Cards
{
    class ProximityFuse : CustomCard
    {

        public static CardInfo CardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 0.8f;
            gun.reloadTime = 1.2f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            GameObject obj = new GameObject("ProximityFuseEffects", typeof(ProximityFuseEffect));

            ProximityFuseEffect.ownerID = player.playerID;

            (GameObject AddToProjectile, GameObject effect, Explosion explosion) = BreadCards.LoadExplosion("explosionClusterBomb");


            ProximityFuseEffect.ExplosionToSpawn = new ObjectsToSpawn
            {
                AddToProjectile = AddToProjectile,
                direction = ObjectsToSpawn.Direction.forward,
                effect = effect,
                normalOffset = 0.1f,
                scaleFromDamage = 1f,
                scaleStackM = 1f,
                scaleStacks = true,
                spawnAsChild = false,
                spawnOn = ObjectsToSpawn.SpawnOn.all,
                stacks = 0,
                stickToAllTargets = false,
                stickToBigTargets = false,
                zeroZ = false
            };
            effect.GetOrAddComponent<SpawnedAttack>().spawner = player;

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
            return "Proximity Fuse";
        }
        protected override string GetDescription()
        {
            return "Your bullets explode when near enemies";
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
                new CardInfoStat()
                {
                    positive = false,
                    stat = "DMG",
                    amount = "-20%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                                new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload Time",
                    amount = "+20%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
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

    public class ProximityFuseEffect : MonoBehaviour
    {
        public static int ownerID;

        public Player owner;

        public Explosion explosion;
        public static ObjectsToSpawn ExplosionToSpawn;

        private MoveTransform moveTransform;
        private PhotonView photonView;

        public void Awake()
        {
            if (transform.parent != null)
            {
                transform.parent.gameObject.AddComponent<ProximityFuseEffect>();
                return;
            }
            else
            {
                foreach (var obj in GetComponentsInChildren<ProximityFuseEffect>().Where(bullet => bullet != this))
                {
                    Destroy(obj.gameObject);
                }
            }
            this.ExecuteAfterSeconds(0.5f, () =>
            {
                explosion = new Explosion();
                start = true;
                photonView = GetComponent<PhotonView>();
                moveTransform = GetComponent<MoveTransform>();

                while (owner == null) { owner = GetComponent<SpawnedAttack>().spawner; }

            });

        }
        bool start;
        public void Update()
        {
            if (!start) return;

            if (photonView != null)
            {
                Player target = PlayerManager.instance.GetClosestPlayer(transform.position, true);

                if (BreadCards.Distance(transform.position, target.transform.position) > 25f)
                {
                    Explode(transform.position);
                }
            }
        }
        public static SoundEvent sound;
        private void Explode(Vector2 position)
        {
            sound = owner.data.playerSounds.soundCharacterLand;

            // spawn explosion near bullet hit
            GameObject ex = Instantiate(ExplosionToSpawn.effect, position, Quaternion.identity);

            // delete explosion after 2s
            Destroy(ex, 2); // TODO: Doesn't work

            Destroy(this, 2);

            // sound
            SoundManager.Instance.Play(sound, transform); // TODO: new transform from Vector2 position?
        }

    }
}
*/