using Core;
using Managers.Controllers;
using Managers.Interfaces;
using Tools;
using UnityEngine;
using static Settings.Enumerators;

namespace Player
{
    public class PlayerPresenter : SimplePresenter<PlayerPresenter, PlayerPresenterView>
    {
        private  float MaxVelocityHorizontal =>_playerData.maxVelocityHorizontal;
        private  float JumpPower =>_playerData.jumpPower;
        private  float MaxHealth =>_playerData.maxHealth;
        private  float DamagePower =>_playerData.damagePower;

        private float _currentHealth;
        private bool _isAttack;
        private bool _isProtect;

        private readonly GameInputController _gameInputController;
        private readonly PlayerData _playerData;

        public PlayerPresenter(PlayerPresenterView view) : base(view)
        {
            _gameInputController = GameClient.Get<IGameplayManager>().GetController<GameInputController>();
            
            _playerData = GameClient.Get<IDataManager>().GetPlayerData();

            _gameInputController.DirectionMoveEvent += SetDirectionMove;
            _gameInputController.JumpEvent += Jump;
            _gameInputController.AttackEvent += Attack;
            _gameInputController.ProtectedEvent += Protected;

            _currentHealth = MaxHealth;
            
            View.SetHpBar(_currentHealth / MaxHealth);

            View.GetDamageEvent += CasusDamage;
            _isAttack = false;
        }

        private void Attack()
        {
            if (_isAttack)
            {
                return;
            }

            _isAttack = true;
            View.SetActiveAttack(true);

            Vector2 attackPos = View.transform.position + (Vector3.up * 2);
            var hit = Physics2D.CircleCast(attackPos, 2f, View.GetDirectionPlayer(), distance: 2.5f, layerMask: 1 << 9);

            if (hit.collider != null)
            {
                var receiver = hit.collider.GetComponent<IDamageReceiver>();

                receiver.GetDamage(DamagePower);
            }

            TaskManager.ExecuteAfterDelay(0.3f, () => View.SetActiveAttack(false));
            TaskManager.ExecuteAfterDelay(0.4f, () => _isAttack = false);
        }

        private void CasusDamage(float damageValue)
        {
            if (_isProtect)
            {
                return;
            }

            _currentHealth -= damageValue;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Deaths();
            }
            else
            {
                View.StartHurt();
            }


            View.SetHpBar(_currentHealth / MaxHealth);
        }

        private void Protected(bool isProtect)
        {
            _isProtect = isProtect;
            View.Protected(isProtect);
        }

        private void Jump()
        {
            if (_isProtect)
            {
                return;
            }

            View.Jump(JumpPower);
        }

        private void Deaths()
        {
            _gameInputController.DirectionMoveEvent -= SetDirectionMove;
            _gameInputController.JumpEvent -= Jump;
            _gameInputController.AttackEvent -= Attack;
            _gameInputController.ProtectedEvent -= Protected;

            View.StartDeaths();

            TaskManager.ExecuteAfterDelay(1f, Destroy);
        }

        private void SetDirectionMove(DirectionMove directionMove)
        {
            if (directionMove == DirectionMove.Undirection)
                View.StopMove();
            else if (directionMove is DirectionMove.Right or DirectionMove.Left)
                View.SetDirectionMove(directionMove, MaxVelocityHorizontal);
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation) => View.SetPositionAndRotation(position, rotation);

        public Vector3 GetPosition() => View.transform.position;
    }
}