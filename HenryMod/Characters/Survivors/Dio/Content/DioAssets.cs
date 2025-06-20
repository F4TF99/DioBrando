using RoR2;
using UnityEngine;
using DioMod.Modules;
using System;
using RoR2.Projectile;

namespace DioMod.Survivors.Henry
{
    public static class DioAssets
    {
        // particle effects
        public static GameObject swordSwingEffect;
        public static GameObject swordHitImpactEffect;

        public static GameObject bombExplosionEffect;

        // networked hit sounds
        public static NetworkSoundEventDef swordHitSoundEvent;

        //projectiles
        public static GameObject bombProjectilePrefab;
        public static GameObject knifeProjectilePrefab;

        public static GameObject standGameObject;
        private static string standGameObjectName = "mdlStand";

        private static AssetBundle _assetBundle;

        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            swordHitSoundEvent = Content.CreateAndAddNetworkSoundEventDef("HenrySwordHit");

            CreateEffects();

            CreateProjectiles();

            CreateStandGameObject();
        }

        #region effects
        private static void CreateEffects()
        {
            CreateBombExplosionEffect();

            swordSwingEffect = _assetBundle.LoadEffect("HenrySwordSwingEffect", true);
            swordHitImpactEffect = _assetBundle.LoadEffect("ImpactHenrySlash");
        }

        private static void CreateBombExplosionEffect()
        {
            bombExplosionEffect = _assetBundle.LoadEffect("BombExplosionEffect", "HenryBombExplosion");

            if (!bombExplosionEffect)
                return;

            ShakeEmitter shakeEmitter = bombExplosionEffect.AddComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.duration = 0.5f;
            shakeEmitter.radius = 200f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 1f,
                frequency = 40f,
                cycleOffset = 0f
            };

        }
        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {
            CreateBombProjectile();
            CreateKnifeProjectile();
            Content.AddProjectilePrefab(bombProjectilePrefab);
        }

        private static void CreateKnifeProjectile()
        {
            knifeProjectilePrefab = Asset.CloneProjectilePrefab("FMJRamping", "CaesarKnifeGhost");
            
            ProjectileDamage damage = knifeProjectilePrefab.GetComponent<ProjectileDamage>();
            ProjectileSimple simple = knifeProjectilePrefab.GetComponent<ProjectileSimple>();
            ProjectileOverlapAttack overlap = knifeProjectilePrefab.GetComponent<ProjectileOverlapAttack>();

            simple.lifetime = 1f;

            overlap.impactEffect = null;
            overlap.onServerHit = null;

            ProjectileController knifeController = knifeProjectilePrefab.GetComponent<ProjectileController>();

            if (_assetBundle.LoadAsset<GameObject>("CaesarKnifeGhost") != null)
                knifeController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("CaesarKnifeGhost");
        }

        //private static void CreateKnifeProjectile()
        //{
        //    knifeProjectilePrefab = Asset.CloneProjectilePrefab("FireMeatBall", "CaesarKnifeGhost");
        //    UnityEngine.Object.Destroy(knifeProjectilePrefab.GetComponent<ProjectileImpactExplosion>());

        //    ProjectileController knifeController = knifeProjectilePrefab.GetComponent<ProjectileController>();
        //    Rigidbody rigidBody = knifeProjectilePrefab.GetComponent<Rigidbody>();
        //    rigidBody.useGravity = false;

        //    if (_assetBundle.LoadAsset<GameObject>("CaesarKnifeGhost") != null)
        //        knifeController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("CaesarKnifeGhost");
        //}

        private static void CreateBombProjectile()
        {
            //highly recommend setting up projectiles in editor, but this is a quick and dirty way to prototype if you want
            bombProjectilePrefab = Asset.CloneProjectilePrefab("CommandoGrenadeProjectile", "HenryBombProjectile");

            //remove their ProjectileImpactExplosion component and start from default values
            UnityEngine.Object.Destroy(bombProjectilePrefab.GetComponent<ProjectileImpactExplosion>());
            ProjectileImpactExplosion bombImpactExplosion = bombProjectilePrefab.AddComponent<ProjectileImpactExplosion>();
            
            bombImpactExplosion.blastRadius = 16f;
            bombImpactExplosion.blastDamageCoefficient = 1f;
            bombImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 12f;
            bombImpactExplosion.impactEffect = bombExplosionEffect;
            bombImpactExplosion.lifetimeExpiredSound = Content.CreateAndAddNetworkSoundEventDef("HenryBombExplosion");
            bombImpactExplosion.timerAfterImpact = true;
            bombImpactExplosion.lifetimeAfterImpact = 0.1f;

            ProjectileController bombController = bombProjectilePrefab.GetComponent<ProjectileController>();

            if (_assetBundle.LoadAsset<GameObject>("HenryBombGhost") != null)
                bombController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("HenryBombGhost");
            
            bombController.startSound = "";
        }
        #endregion projectiles

        #region stand assets
        private static void CreateStandGameObject()
        {
            standGameObject = _assetBundle.LoadAsset<GameObject>(standGameObjectName);
            if (standGameObject == null)
            {
                Log.Error($"DioAssets: Failed to load ${standGameObjectName}");
            }
            else
            {
                Log.Message($"DioAssets: GameObject {standGameObject.name} has successfully been loaded!");
            }
        }
        #endregion
    }
}
