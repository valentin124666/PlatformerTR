using System;
using UnityEngine;

namespace Player
{
    public class SensorView : MonoBehaviour
    {
        private int _colCount = 0;

        private float _disableTimer;

        public event Action EnterTrigger;
        public event Action ExitTrigger;

        private void OnEnable()
        {
            _colCount = 0;
        }

        public bool State()
        {
            return _colCount > 0;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            _colCount++;
            EnterTrigger?.Invoke();
        }

        void OnTriggerExit2D(Collider2D other)
        {
            _colCount--;
            ExitTrigger?.Invoke();
        }


    }
}
