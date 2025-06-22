using DioMod.Characters.Survivors.Dio.Components.Dio;
using EntityStates;
using IL.RoR2;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DioMod.Survivors.Henry.SkillStates
{
    public class Blink : BaseSkillState
    {
        public static float duration = 0.1f;
        public static float initialSpeedCoefficient = 100f;
        public static float finalSpeedCoefficient = 1f;

        private float blinkSpeed;
        private Vector3 forwardDirection;
        private Vector3 previousPosition;

        private StandLinkComponent standLinkComponent;
        private RoR2.CharacterModel characterModel;

        private int initialInvisibilityCount;

        private Renderer standRenderer;
        private Renderer baseObjectRenderer;

        // Finds the forward direction of the user by looking at the default direction and the inputbank
        // Calculates the new blink speed by using Mathf.Lerp to smoothly transition from float A to float B over the specified amount of time
        // Sets what the previous position was so that we can use it to calculate the force/vector used to move the object to its current position
        public override void OnEnter() 
        {
            base.OnEnter();

            standLinkComponent = base.gameObject.GetComponent<StandLinkComponent>();
            Transform modelTransform = base.GetModelTransform();
            characterModel = modelTransform.GetComponent<RoR2.CharacterModel>();

            if (characterModel != null)
            {
                initialInvisibilityCount = characterModel.invisibilityCount;
                characterModel.invisibilityCount = 0;
            }

            if (isAuthority && inputBank && characterDirection)
            {
                forwardDirection = (inputBank.moveVector == Vector3.zero ? characterDirection.forward : inputBank.moveVector);
            }

            RecalculateBlinkSpeed();

            if (characterMotor && characterDirection)
            {
                characterMotor.velocity.y = 0f;
                characterMotor.velocity = forwardDirection * blinkSpeed;
            }

            Vector3 previousForce = characterMotor ? characterMotor.velocity : Vector3.zero;
            previousPosition = transform.position - previousForce;
        }

        // Recalculates the new blink speed
        // Ensures that the character direction consistently stays at the direction the character at was when this skill was first used
        // Recalculate what the new force will be by multiplying the previously applied force to the recalculated blink
        // [I don't understand this part] ----> New force is then used to get the dot product of the new force and the forward direction, to see what will be the force magnitude that goes in a specific direction
        // We then multiply this to the forwardDirection and use that value to set the characterMotor velocity
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            RecalculateBlinkSpeed();

            if (characterDirection) characterDirection.forward = forwardDirection;
            
            Vector3 normalized = (transform.position - previousPosition).normalized;
            if (characterMotor && characterDirection && normalized != Vector3.zero)
            {
                Vector3 vector = normalized * blinkSpeed;
                float d = Mathf.Max(Vector3.Dot(forwardDirection, vector), 0f);
                vector = forwardDirection * d;
                vector.y = 0f;

                characterMotor.velocity = vector;
            }
            previousPosition = transform.position;

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            characterMotor.velocity = Vector3.zero;
            base.OnExit();

            if (characterModel != null)
            {
                characterMotor.disableAirControlUntilCollision = false;
                characterModel.invisibilityCount = initialInvisibilityCount;
            }
        }

        private void RecalculateBlinkSpeed()
        {
            blinkSpeed = moveSpeedStat * Mathf.Lerp(initialSpeedCoefficient, finalSpeedCoefficient, fixedAge / duration);
        }
    }
}
