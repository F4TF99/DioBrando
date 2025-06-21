using DioMod.Characters.Survivors.Dio.Components.Dio;
using DioMod.Survivors.Henry;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DioMod.Characters.Survivors.Dio.Components.Stand
{
    public class StandController : MonoBehaviour
    {
        GameObject dioGameObject;
        CharacterDirection dioCharacterDirection;
        CharacterBody dioCharacterBody;

        StandLinkComponent standLinkComponent;

        System.Random random;

        private StandState state = StandState.Idle;

        #region class members for debugging
        bool debugMode = true;

        bool stateLogPrinted = false;
        #endregion

        #region class members for attacks
        private float prepareBarrageStopwatchFixed = 0.3f;
        private float prepareBarrageStopwatch;
        bool hasBarrageBeenPrepared;

        private float barrageStopwatchFixed = 0.1f;
        private float barrageStopwatch;
        private int barrageAttackCounter = 0;
        private int barrageAttackCounterLimit = 20;
        #endregion

        private void Awake() 
        { 
            prepareBarrageStopwatch = prepareBarrageStopwatchFixed;
            barrageStopwatch = barrageStopwatchFixed;
            random = new System.Random();
        }

        private void Update() 
        {
            var aimRay = dioCharacterBody.inputBank.GetAimRay();

            if (debugMode && !stateLogPrinted) Log.Message($"StandController: Stand is in {GetState()} state"); stateLogPrinted = true;

            //if (state == StandState.Idle)
            if (state != null)
            {
                base.transform.position = dioGameObject.transform.position + new Vector3(1f, 0.2f, 1f);
                base.transform.forward = dioCharacterDirection.forward;
            }

            if (state == StandState.MovingToBarrage)
            {
                prepareBarrageStopwatch -= Time.deltaTime;

                if (prepareBarrageStopwatch <= 0)
                {
                    if (debugMode) Log.Message($"StandController: Stand is transitioning from {GetState()} to Barrage state"); stateLogPrinted = false;
                    state = StandState.Barrage;
                    prepareBarrageStopwatch = prepareBarrageStopwatchFixed;
                }
            }
            else if (state == StandState.Barrage)
            {
                barrageStopwatch -= Time.deltaTime;

                if (barrageStopwatch <= 0)
                {
                    var force = (barrageAttackCounter >= barrageAttackCounterLimit) ? 8000f : 500f;
                    FireBarrageAttack(aimRay, force);

                    if (barrageAttackCounter >= barrageAttackCounterLimit)
                    {
                        if (debugMode) Log.Message($"StandController: Stand is transitioning from {GetState()} to BarrageFinished state"); stateLogPrinted = false;
                        state = StandState.BarrageFinished;
                        barrageAttackCounter = 0;
                    }
                    else
                    {
                        barrageAttackCounter++;
                    }

                    barrageStopwatch = barrageStopwatchFixed;
                }
            }
        }

        private void FireBarrageAttack(Ray _aimRay, float _force)
        {
            float randomX = random.Next(-2, 2);
            float randomY = random.Next(-2, 2);
            float randomZ = random.Next(-2, 2);

            FireProjectileInfo barrageProjectile = new FireProjectileInfo
            {
                projectilePrefab = DioAssets.standLeftHandPrefab,
                position = base.transform.position + new Vector3(randomX, randomY, randomZ),
                rotation = Util.QuaternionSafeLookRotation(_aimRay.direction),
                force = _force,
                damage = 1f,
                crit = false,
                owner = dioGameObject,
                target = null,
                damageTypeOverride = DamageType.Generic
            };

            ProjectileManager.instance.FireProjectile(barrageProjectile);
        }

        public string GetState() 
        { 
            switch (state)
            {
                case StandState.Idle:
                    return "Idle";
                case StandState.MovingToBarrage:
                    return "MovingToBarrage";
                case StandState.Barrage:
                    return "Barrage";
                case StandState.BarrageFinished:
                    return "BarrageFinished";
                default:
                    return "None";
            }
        }
        public void SetState(string _state) 
        { 
            switch (_state.ToLower().Trim())
            {
                case "idle":
                    state = StandState.Idle;
                    break;
                case "movingtobarrage":
                    state = StandState.MovingToBarrage;
                    break;
                case "barrage":
                    state = StandState.Barrage;
                    break;
                case "barragefinished":
                    state = StandState.BarrageFinished;
                    break;
                default:
                    return;
            }
        }
        public void SetDioGameObject(GameObject _dioGameObject) {
            dioGameObject = _dioGameObject;
            dioCharacterDirection = dioGameObject.GetComponent<CharacterDirection>();
            standLinkComponent = dioGameObject.GetComponent<StandLinkComponent>();
            dioCharacterBody = dioGameObject.GetComponent<CharacterBody>();
        }

        private enum StandState
        {
            Idle,
            MovingToBarrage,
            Barrage,
            BarrageFinished
        }
    }
}
