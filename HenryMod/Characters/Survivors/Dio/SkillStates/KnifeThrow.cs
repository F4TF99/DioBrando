using DioMod.Modules;
using DioMod.Survivors.Henry;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DioMod.Survivors.Henry.SkillStates
{
    public class KnifeThrow : GenericProjectileBaseState
    {
        float fireTime = 0.1f;
        float duration = 0.7f;

        bool knifeThrown = false;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= fireTime && !knifeThrown)
            {
                ThrowKnife();
                knifeThrown = true;
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
         
        public override void OnExit()
        {
            base.OnExit();
        }

        private void ThrowKnife()
        {
            Ray aimRay = base.GetAimRay();

            if (base.isAuthority)
            {
                FireProjectileInfo knifeProjectile = new FireProjectileInfo
                {
                    projectilePrefab = DioAssets.knifeProjectilePrefab,
                    position = base.transform.position,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    force = 1f,
                    damage = 1f,
                    crit = base.RollCrit(),
                    owner = base.gameObject,
                    target = null,
                    damageTypeOverride = DamageType.BleedOnHit
                };

                ProjectileManager.instance.FireProjectile(knifeProjectile);
            }
        }
    }
}
