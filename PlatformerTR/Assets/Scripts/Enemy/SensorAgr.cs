using System;
using Player;
using Settings;
using UnityEngine;

namespace Enemy
{
    public class SensorAgr : MonoBehaviour
    {
        [SerializeField] private Enumerators.EntityType whoIsAggro;

        public event Action<IDamageReceiver> Spotted;
        public event Action<IDamageReceiver> Unnoticed;

        private void OnTriggerEnter2D(Collider2D col)
        {
            var receiver = col.GetComponent<IDamageReceiver>();

            if (receiver != null && receiver.EntityType == whoIsAggro)
            {
                Spotted?.Invoke(receiver);
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            var receiver = col.GetComponent<IDamageReceiver>();

            if (receiver != null && receiver.EntityType == whoIsAggro)
            {
                Unnoticed?.Invoke(receiver);
            }
        }
    }
}