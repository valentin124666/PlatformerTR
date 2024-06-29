using System;
using Core;
using Player;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    [PrefabInfo(Enumerators.NamePrefabAddressable.EnemyGolem)]
    public class EnemyPresenterView : SimplePresenterView<EnemyPresenter, EnemyPresenterView>, IDamageReceiver
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform modelTransform;
        [SerializeField] private SensorAgr sensorAgr;
        [SerializeField] private Collider2D mainCollider;
        [SerializeField] private Image hpBar;

        private Vector3 _targetMove;
        private IDamageReceiver _damageReceiver;
        public Vector3 PositionReceiver => transform.position;
        public event Action<float> GetDamageEvent;

        public SensorAgr SensorAgr => sensorAgr;

        private bool _isDestroyed;
        private bool _isMove;
        private bool _isAttack;
        private float _moveSpeed;

        private static readonly int MoveHash = Animator.StringToHash("Move");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int DeathHash = Animator.StringToHash("Death");

        public event Action EndMovetoTarget;
        public Enumerators.EntityType EntityType => Enumerators.EntityType.Enemy;

        public override void Init()
        {
        }

        private float TurnModelToTarget(Vector3 target)
        {
            var currentPosition = modelTransform.position;
            var targetPosition = target;
            var distanceVector = targetPosition - currentPosition;

            var sqrMagnitude = distanceVector.sqrMagnitude;

            var targetRotation = sqrMagnitude > 0.0049f ? Quaternion.LookRotation(distanceVector, Vector3.up) : modelTransform.rotation;

            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, 0.3f);
            return sqrMagnitude;
        }

        private void FixedUpdate()
        {
            if (_isMove)
            {
                TurnModelToTarget(_targetMove);
                transform.position = Vector3.MoveTowards(transform.position, _targetMove, _moveSpeed);

                if ((transform.position - _targetMove).sqrMagnitude < 0.0025f)
                {
                    _isMove = false;
                    animator.SetBool(MoveHash, false);
                    EndMovetoTarget?.Invoke();
                }
            }

            if (_isAttack)
            {
                if (_damageReceiver != null)
                {
                    var turnTarget = modelTransform.position;
                    turnTarget.x = _damageReceiver.PositionReceiver.x;

                    var sqrMagnitude = TurnModelToTarget(turnTarget);
                    if (sqrMagnitude < 0.0049f)
                    {
                        _isAttack = false;
                        _damageReceiver = null;
                    }
                }
            }
        }

        public void StartAttack(IDamageReceiver receiver)
        {
            _damageReceiver = receiver;
            _isAttack = true;
            animator.SetTrigger(AttackHash);
        }
        public void StartDeaths()
        {
            _isDestroyed = true;
            mainCollider.enabled = false;
            animator.SetTrigger(DeathHash);
        }
        public void StopAttack()
        {
            _damageReceiver = null;
            _isAttack = false;
        }

        public void SetMoveSpeed(float speed)
        {
            _moveSpeed = speed;
        }

        public void SetTargetMove(Vector3 target)
        {
            if (_isDestroyed)
            {
                return;
            }

            _targetMove = target;
            _isMove = true;
            animator.SetBool(MoveHash, true);
        }

        public void StopMove()
        {
            _isMove = false;
            animator.SetBool(MoveHash, false);
            EndMovetoTarget?.Invoke();
        }

        public bool CheckCollider(Collider2D collider2D)
        {
            return collider2D == mainCollider;
        }

        public void SetHpBar(float value)
        {
            hpBar.fillAmount = value;
        }

        public void GetDamage(float damageValue)
        {
            GetDamageEvent?.Invoke(damageValue);
        }

        public Vector3 GetDirectionModel() => modelTransform.forward;

        public Vector3 GetPositionModel() => modelTransform.position;

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation) => transform.SetPositionAndRotation(position, rotation);
    }
}