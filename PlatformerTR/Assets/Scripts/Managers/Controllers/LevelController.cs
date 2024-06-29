using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Level;
using Managers.Interfaces;
using Settings;
using UnityEngine;

namespace Managers.Controllers
{
    public class LevelController : IController
    {
        private LevelPresenter _levelPresenter;

        private GameObject _levelResource;
        private Transform _poolLevel;
        public bool IsInit { get; private set; }

        void IController.Init()
        {
            _poolLevel = new GameObject("[PoolLevel]").transform;
            CreateLevelPresenters().Forget();
        }


        private async UniTask CreateLevelPresenters()
        {
            _levelPresenter = await ResourceLoader.Instantiate<LevelPresenter, LevelPresenterView>(_poolLevel, "");
            _levelPresenter.SetActive(false);
            IsInit = true;
        }

        public void ActivationLevel()
        {
            _levelPresenter.SetActive(true);
        }

        public void GetStartPositionAndRotationPlayer(out Vector3 position, out Quaternion rotation)
        {
            if (_levelPresenter == null)
            {
                throw new NullReferenceException();
            }

            _levelPresenter.GetStartPositionAndRotationPlayer(out position, out rotation);
        }  
        public void GetStartPositionAndRotationCamera(out Vector3 position, out Quaternion rotation)
        {
            if (_levelPresenter == null)
            {
                throw new NullReferenceException();
            }

            _levelPresenter.GetStartPositionAndRotationCamera(out position, out rotation);
        }

        public IEnumerable<ConfigurationEnemy> GetConfigurationEnemy()
        {
            if (_levelPresenter == null)
            {
                throw new NullReferenceException();
            }

            return _levelPresenter.GetConfigurationEnemy();
        }

        public void Dispose()
        {
            _levelPresenter?.Destroy();
        }

        public void ResetAll()
        {
        }
    }
}