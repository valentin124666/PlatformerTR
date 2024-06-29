using Core;
using Cysharp.Threading.Tasks;
using Level;
using Managers.Interfaces;
using Player;
using UnityEngine;

namespace Managers.Controllers
{
    public class PlayerController : IController
    {
        private Transform _poolPlayer;
        private LevelController _levelController;
        private PlayerPresenter _playerPresenter;
        
        public bool IsInit { get; private set; }

        void IController.Init()
        {
            _poolPlayer = new GameObject("[PoolPlayer]").transform;
            _levelController = GameClient.Get<IGameplayManager>().GetController<LevelController>();
            CreatePlayerPresenters().Forget();
        }

        private async UniTask CreatePlayerPresenters()
        {
            _playerPresenter = await ResourceLoader.Instantiate<PlayerPresenter, PlayerPresenterView>(_poolPlayer, "");
            _playerPresenter.SetActive(false);

            IsInit = true;
        }

        public void ActivationPlayer()
        {
            _playerPresenter.SetActive(true);
        }

        public void MovePlayerToStartPosition()
        {
            _levelController.GetStartPositionAndRotationPlayer(out var position, out var rotation);
            _playerPresenter.SetPositionAndRotation(position,rotation);
        }

        public Vector3 GetPositionPlayer() => _playerPresenter.GetPosition();

        public void Dispose()
        {
            _playerPresenter?.Destroy();
        }

        public void ResetAll()
        {
        }
    }
}