using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using Level;
using Managers;
using Managers.Interfaces;
using Player;
using Tools;
using UnityEngine;

namespace Enemy
{
    public class EnemyPresenter : SimplePresenter<EnemyPresenter, EnemyPresenterView>
    {
        private Vector3[] _patrolPoints;

        private TriggerZone _triggerZone;
        private CancellationTokenSource _attackToken;

        private EnemyData _enemyData;

        private float MoveSpeed => _enemyData.moveSpeed;
        private float DamagePower => _enemyData.damagePower;
        private float MaxHealth => _enemyData.maxHealth;

        private float _currentHealth;
        private int _currentPatrolPoints;
        private bool _isGoBack;


        public EnemyPresenter(EnemyPresenterView view) : base(view)
        {
            View.SensorAgr.Spotted += StartAttack;
            View.SensorAgr.Unnoticed += StopAttack;
            _enemyData = GameClient.Get<IDataManager>().GetEnemyData();

            _currentHealth = MaxHealth;

            View.SetHpBar(_currentHealth / MaxHealth);
            View.GetDamageEvent += CasusDamage;
        }

        private void CasusDamage(float damageValue)
        {
            _currentHealth -= damageValue;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Deaths();
            }

            View.SetHpBar(_currentHealth / MaxHealth);
        }

        private void Deaths()
        {
            View.SensorAgr.Spotted -= StartAttack;
            View.SensorAgr.Unnoticed -= StopAttack;
            View.EndMovetoTarget -= ContinuePatrol;
            StopAttack();

            View.StartDeaths();
            TaskManager.ExecuteAfterDelay(1.3f, Destroy);
        }

        private void ContinuePatrol()
        {
            if (!_isGoBack)
            {
                _currentPatrolPoints++;
                if (_currentPatrolPoints >= _patrolPoints.Length - 1)
                {
                    _isGoBack = true;
                    _currentPatrolPoints = _patrolPoints.Length - 1;
                }
            }
            else
            {
                _currentPatrolPoints--;
                if (_currentPatrolPoints <= 0)
                {
                    _isGoBack = false;
                    _currentPatrolPoints = 0;
                }
            }

            View.SetTargetMove(_patrolPoints[_currentPatrolPoints]);
        }

        private void StopAttack(IDamageReceiver receiver)
        {
            StopAttack();
        }

        private void StopAttack()
        {
            _attackToken?.Cancel();
            _attackToken?.Dispose();
            _attackToken = null;
        }

        private void StartAttack(IDamageReceiver receiver)
        {
            View.EndMovetoTarget -= ContinuePatrol;
            View.StopMove();

            StopAttack();

            _attackToken = new CancellationTokenSource();
            AttackFlow(receiver).Forget();
        }

        private async UniTask AttackFlow(IDamageReceiver receiver)
        {
            try
            {
                while (true)
                {
                    View.StartAttack(receiver);
                    await TaskManager.WaitUntilDelay(1f, _attackToken.Token);
                    Vector2 attackPos = View.GetPositionModel() + (Vector3.up * 4f);

                    var hit = Physics2D.CircleCast(attackPos, 2f, View.GetDirectionModel(), distance: 4f, layerMask: 1 << 8);

                    if (hit.collider != null)
                    {
                        if (receiver.CheckCollider(hit.collider))
                        {
                            receiver.GetDamage(DamagePower);
                        }
                    }

                    await TaskManager.WaitUntilDelay(1f, _attackToken.Token);
                }
            }
            finally
            {
                if (View != null && !IsDestroyed)
                {
                    View.StopAttack();
                    StartPatrolPoints();
                }
            }
        }

        private void StartPatrolPoints()
        {
            View.SetMoveSpeed(MoveSpeed);
            View.SetTargetMove(_patrolPoints[_currentPatrolPoints]);
            View.EndMovetoTarget += ContinuePatrol;
        }

        public void SetPatrol(TriggerZone triggerZone, IEnumerable<Vector3> patrolPoints)
        {
            if (_triggerZone != null)
            {
                _triggerZone.Triggered -= StartPatrolPoints;
            }

            _triggerZone = triggerZone;
            _triggerZone.Triggered += StartPatrolPoints;

            _patrolPoints = patrolPoints.ToArray();
            _currentPatrolPoints = 0;
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation) => View.SetPositionAndRotation(position, rotation);

        public override void Destroy()
        {
            base.Destroy();
            _attackToken?.Cancel();
        }
    }
}