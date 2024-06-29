using System;
using Core;
using Managers.Controllers;
using Managers.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static Settings.Enumerators;
using Random = UnityEngine.Random;

namespace Player
{
    [PrefabInfo(NamePrefabAddressable.HeroKnight)]
    public class PlayerPresenterView : SimplePresenterView<PlayerPresenter, PlayerPresenterView>, IDamageReceiver
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rigidbody2d;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SensorModules sensorModules;
        [SerializeField] private Collider2D mainCollider;
        [SerializeField] private Image hpBar;

        private Vector3 _directionPlayer;
        private int _jumpCount;

        public EntityType EntityType => EntityType.Player;
        public Vector3 PositionReceiver => transform.position;
        public event Action<float> GetDamageEvent;

        private Vector2 MainVelocity
        {
            get => rigidbody2d.velocity;
            set => rigidbody2d.velocity = value;
        }

        private Vector2 _targetVelocity;

        private int _randomAnimationAttack;
        private bool _isAttack;
        private bool _isProtected;

        private static readonly int IdleBlockHash = Animator.StringToHash("IdleBlock");
        private static readonly int BlockHash = Animator.StringToHash("Block");
        private static readonly int RunSpeedHash = Animator.StringToHash("RunSpeed");
        private static readonly int GroundedHash = Animator.StringToHash("Grounded");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int AirSpeedYHash = Animator.StringToHash("AirSpeedY");
        private static readonly int WallSlideHash = Animator.StringToHash("WallSlide");
        private static readonly int HurtHash = Animator.StringToHash("Hurt");
        private static readonly int DeathHash = Animator.StringToHash("Death");

        public bool IsStandingOnGround { get; private set; }

        public override void Init()
        {
            sensorModules.StandingOnGroundEvent += SetStateGroundAnimator;
            sensorModules.WallSlideRightEvent += SetStateWallSlideAnimator;
        }

        private void FixedUpdate()
        {
            if (!IsStandingOnGround)
            {
                animator.SetFloat(AirSpeedYHash, MainVelocity.y);
            }

            if (_isAttack || _isProtected)
            {
                if (IsStandingOnGround)
                {
                    var currentVelocity = MainVelocity;
                    currentVelocity.x = 0;
                    MainVelocity = currentVelocity;
                }

                return;
            }

            if (Mathf.Abs(_targetVelocity.x - MainVelocity.x) >= 0.01f)
            {
                _targetVelocity.y = MainVelocity.y;
                MainVelocity = Vector3.Lerp(MainVelocity, _targetVelocity, 0.4f);
            }
        }

        private void SetStateWallSlideAnimator(bool isWallSlide)
        {
            animator.SetBool(WallSlideHash, isWallSlide);
        }

        private void SetStateGroundAnimator(bool isGround)
        {
            if (isGround)
            {
                _jumpCount = 2;
            }
            IsStandingOnGround = isGround;

            animator.SetFloat(AirSpeedYHash, MainVelocity.y);

            animator.SetBool(GroundedHash, isGround);
        }

        private void Attack()
        {
            if (IsStandingOnGround)
            {
                while (true)
                {
                    var number = Random.Range(1, 3);

                    if (_randomAnimationAttack == number) continue;

                    _randomAnimationAttack = number;
                    break;
                }
            }
            else
            {
                _randomAnimationAttack = 3;
            }

            animator.SetTrigger("Attack" + _randomAnimationAttack);
        }

        public void Protected(bool isProtected)
        {
            _isProtected = isProtected;
            if (isProtected)
            {
                animator.SetTrigger(BlockHash);
            }

            animator.SetBool(IdleBlockHash, isProtected);
        }

        public void SetActiveAttack(bool active)
        {
            if (active.Equals(_isAttack))
            {
                return;
            }

            if (active)
            {
                Attack();
            }

            _isAttack = active;
        }

        public void StartHurt()
        {
            animator.SetTrigger(HurtHash);
        }

        public void StartDeaths()
        {
            sensorModules.StandingOnGroundEvent -= SetStateGroundAnimator;
            sensorModules.WallSlideRightEvent -= SetStateWallSlideAnimator;
            mainCollider.gameObject.layer = 0;
            animator.SetTrigger(DeathHash);
        }

        public void Jump(float jumpPower)
        {
            if (_jumpCount<=0)
            {
                return;
            }

            _jumpCount--;
            var currentVelocity = MainVelocity;
            currentVelocity.y = jumpPower;
            MainVelocity = currentVelocity;

            animator.SetFloat(AirSpeedYHash, MainVelocity.y);
            animator.SetTrigger(JumpHash);
        }

        public void StopMove()
        {
            _targetVelocity = Vector2.zero;
            animator.SetInteger(RunSpeedHash, 0);
        }

        public Vector2 GetDirectionPlayer() => _directionPlayer;

        public void SetDirectionMove(DirectionMove directionMove, float maxVelocityHorizontal)
        {
            _directionPlayer=Vector2.zero;
            switch (directionMove)
            {
                case DirectionMove.Right:
                    _targetVelocity = Vector2.right * maxVelocityHorizontal;
                    spriteRenderer.flipX = false;
                    animator.SetInteger(RunSpeedHash, 1);
                    _directionPlayer = Vector2.right;
                    break;
                case DirectionMove.Left:
                    _targetVelocity = Vector2.left * maxVelocityHorizontal;
                    spriteRenderer.flipX = true;
                    animator.SetInteger(RunSpeedHash, 1);
                    _directionPlayer = Vector2.left;
                    break;
            }
        }

        public void SetHpBar(float value)
        {
            hpBar.fillAmount = value;
        }

        public bool CheckCollider(Collider2D collider)
        {
            return collider == mainCollider;
        }

        public void GetDamage(float damageValue)
        {
            GetDamageEvent?.Invoke(damageValue);
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            rigidbody2d.velocity = Vector2.zero;
            transform.SetPositionAndRotation(position, rotation);
        }
    }
}