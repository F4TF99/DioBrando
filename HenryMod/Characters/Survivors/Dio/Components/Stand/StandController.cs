using RoR2;
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

        private StandState state = StandState.Idle;

        private void Awake() { }

        private void Update() 
        {
            if (state == StandState.Idle)
            {
                base.transform.position = dioGameObject.transform.position + new Vector3(1f, 0.2f, 1f);
                base.transform.forward = dioCharacterDirection.forward;

            }
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
                default:
                    return;
            }
        }
        public void SetDioGameObject(GameObject _dioGameObject) {
            dioGameObject = _dioGameObject;
            dioCharacterDirection = dioGameObject.GetComponent<CharacterDirection>();
        }

        private enum StandState
        {
            Idle,
            MovingToBarrage,
            Barrage
        }
    }
}
