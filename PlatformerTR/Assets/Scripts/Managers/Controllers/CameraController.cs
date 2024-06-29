using Core;
using Managers.Interfaces;
using UnityEngine;

namespace Managers.Controllers
{
    public class CameraController : IController
    {
        public bool IsInit { get; }

        private Vector3 _offset;
        private Transform _cameraTransform;
        private Camera _mainCamera;
        private PlayerController _playerController;

        private bool _isActiveTrack;
        private Vector3 _velocity;
        private readonly float _cameraMovementSpeed = 0.3f;

        public void Init()
        {
            _playerController = GameClient.Get<IGameplayManager>().GetController<PlayerController>();
            _mainCamera = Camera.main;
            _cameraTransform = _mainCamera.transform;

            MainApp.Instance.FixedUpdateEvent += UpdateTrackCamera;
        }
        
        private Vector3 MoveCamera()
        {
            var currentPosCamera = (_playerController.GetPositionPlayer() + _offset);
            return Vector3.SmoothDamp(_cameraTransform.position, currentPosCamera, ref _velocity, _cameraMovementSpeed);
        }
        
        private void UpdateTrackCamera()
        {
            if (!_isActiveTrack)
            {
                return;
            }

            _cameraTransform.position = MoveCamera();
        }

        public void ActivationCameraTrack()
        {
            _offset = _cameraTransform.position - _playerController.GetPositionPlayer();
            
            _isActiveTrack = true;
        }
        
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation) => _cameraTransform.SetPositionAndRotation(position, rotation);

        public void Dispose()
        {
        
        }

        public void ResetAll()
        {
        
        }
    }
}
