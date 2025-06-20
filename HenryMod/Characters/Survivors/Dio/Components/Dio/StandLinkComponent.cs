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

        private void SetupStand() {
            try
            {
                standGameObject = Instantiate(DioAssets.standGameObject, gameObject.transform.position, Quaternion.identity);
                standGameObject.SetActive(true);
                standGameObject.name = standGameObjectName;

                StandController standController = standGameObject.AddComponent<StandController>();
                standController.SetDioGameObject(base.gameObject);
            }
            catch (Exception e)
            {
                Log.Error("StandLinkComponent: " + e);
            }
        }
    }
}
