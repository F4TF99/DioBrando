using DioMod.Characters.Survivors.Dio.Components.Dio;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace DioMod.Survivors.Henry.SkillStates
{
    public class BarrageAttack : GenericProjectileBaseState
    {
        StandLinkComponent standLinkComponent;

        int startupDuration;

        public override void OnEnter()
        {
            base.OnEnter();

            standLinkComponent = base.GetComponent<StandLinkComponent>();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Change the state of the stand
            // Continuously send the Link Component the current AimRay
            // Wait for the state of the stand to change, once it changes, exit OR..
            // if the button related to this attack is left go and the stand is still in barrage state
            // exit as well

            string standState = standLinkComponent.GetStandState();

            if (standState.Equals("Idle"))
            {
                Log.Message($"BarrageAttack: Calling 'SetStandState' to change state to 'MovingToBarrage'");
                standLinkComponent.SetStandState("MovingToBarrage");
            }
            else
            {
                Log.Message($"BarrageAttack: Sending AimRay to StandLinkComponent");
                //standLinkComponent.SetAimRay(base.GetAimRay());
                standLinkComponent.SetAimRayDirection(base.GetAimRay().direction);
            }

            if (standState.Equals("BarrageFinished"))
            {
                Log.Message($"BarrageAttack: Calling 'SetStandState' to change state to 'Idle' since state was in 'BarrageFinished'");
                standLinkComponent.SetStandState("Idle");
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            Log.Message("BarrageAttack: Exiting attack");
            base.OnExit();
        }
    }
}
