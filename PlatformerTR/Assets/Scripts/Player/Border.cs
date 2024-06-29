using System;
using Managers.Controllers;
using Managers.Interfaces;
using UnityEngine;

namespace Player
{
    public class Border : MonoBehaviour
    {
        private PlayerController _playerController;
        
        void Start()
        {
            _playerController = GameClient.Get<IGameplayManager>().GetController<PlayerController>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                _playerController.MovePlayerToStartPosition();
            }
        }
    }
}
