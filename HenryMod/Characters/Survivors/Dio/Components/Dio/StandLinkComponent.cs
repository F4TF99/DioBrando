using DioMod.Characters.Survivors.Dio.Components.Stand;
using DioMod.Survivors.Henry;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DioMod.Characters.Survivors.Dio.Components.Dio
{
    public class StandLinkComponent : MonoBehaviour
    {
        public GameObject standGameObject;
        private string standGameObjectName = "TheWorld";

        StandController standController;

        private Ray aimRay;
        private Vector3 aimPosition;

        private void Awake()
        {
            if (standGameObject == null)
            {
                SetupStand();
            }
        }

        private void Update()
        {
            if (standGameObject == null)
            {
                SetupStand();
            }
        }

        private void SetupStand() 
        {
            try
            {
                standGameObject = Instantiate(DioAssets.standGameObject, gameObject.transform.position, Quaternion.identity);
                standGameObject.SetActive(true);
                standGameObject.name = standGameObjectName;

                standController = standGameObject.AddComponent<StandController>();
                standController.SetDioGameObject(base.gameObject);
            }
            catch (Exception e)
            {
                Log.Error("StandLinkComponent: " + e);
            }
        }

        public void SetAimRay(Ray _aimRay)
        {
            aimRay = _aimRay;
        }

        public Ray GetAimRay()
        {
            return aimRay;
        }

        public void SetAimRayDirection(Vector3 direction)
        {
            aimPosition = direction;
        }

        public Vector3 GetAimRayDirection()
        {
            return aimPosition;
        }

        public void SetStandState(string state)
        {
            var validStandStatesToTransitionFrom = new List<string>() {
                    "idle",
                    "barragefinished"
                };

            state = state.ToLower().Trim();
            string currentStandState = standController.GetState().ToLower().ToString();

            if (validStandStatesToTransitionFrom.Contains(currentStandState))
            {
                standController.SetState(state);
            }
            else
            {
                Log.Warning($"StandLinkComponent: Can not change Stand state to {state} when Stand is in {GetStandState()} state");
            }
        }

        public string GetStandState()
        {
            return standController.GetState();
        }


    }
}
