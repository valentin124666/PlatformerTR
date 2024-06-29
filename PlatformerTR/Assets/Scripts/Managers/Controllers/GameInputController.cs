using System;
using Core;
using Managers.Interfaces;
using UnityEngine;
using DirectionMove = Settings.Enumerators.DirectionMove;

namespace Managers.Controllers
{
    public class GameInputController : IController
    {
        public bool IsInit { get; }

        public event Action<DirectionMove> DirectionMoveEvent;
        public event Action JumpEvent;
        public event Action AttackEvent;
        public event Action<bool> ProtectedEvent;
        public event Action SomersaultEvent;

        void IController.Init()
        {
            MainApp.Instance.UpdateEvent += UpdateInput;
        }

        private void UpdateInput()
        {
            if ( Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                DirectionMoveEvent?.Invoke(DirectionMove.Right);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                DirectionMoveEvent?.Invoke(DirectionMove.Left);
            }
            else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow))
                {
                    DirectionMoveEvent?.Invoke(DirectionMove.Undirection);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                JumpEvent?.Invoke();
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                AttackEvent?.Invoke();
            }
            else if  (Input.GetMouseButtonDown(1))
            {
                ProtectedEvent?.Invoke(true);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                ProtectedEvent?.Invoke(false);

            }
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                SomersaultEvent?.Invoke();
            }
        }

        public void Dispose()
        {
            
        }

        public void ResetAll()
        {
        }
    }
}