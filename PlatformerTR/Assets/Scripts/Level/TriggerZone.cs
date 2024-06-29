using System;
using Player;
using UnityEngine;

namespace Level
{
    public class TriggerZone : MonoBehaviour
    {
        [SerializeField] private bool multipleUses;
        public event Action Triggered;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.GetComponent<PlayerPresenterView>())
            {
                Triggered?.Invoke();
                if (!multipleUses)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
