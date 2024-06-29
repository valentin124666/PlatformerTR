using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Cysharp.Threading.Tasks;
using Managers.Controllers;
using Managers.Interfaces;
using Settings;
using UI;

namespace Managers
{
    public class GameplayManager : IService, IGameplayManager
    {
        private IUIManager _uIManager;
        private HashSet<IController> _controllers;
        public Enumerators.AppState CurrentState { get; private set; }
        public bool IsPause { get; private set; }
        public bool EndGame { get; private set; }

        public void Dispose()
        {
            foreach (var item in _controllers)
                item.Dispose();
        }

        public T GetController<T>() where T : IController
        {
            return (T)_controllers.First(controller => controller is T);
        }

        public async UniTask Init()
        {
            _uIManager = GameClient.Get<IUIManager>();

            FillControllers();
        }

        private void FillControllers()
        {
            _controllers = new HashSet<IController>()
            {
                new LevelController(),
                new GameInputController(),
                new PlayerController(),
                new EnemyController(),
                new CameraController(),
            };

            foreach (var item in _controllers)
                item.Init();
        }

        public void EnablePause()
        {
            IsPause = true;
        }

        public void RefreshGameplay()
        {
            StopGameplay();
            _uIManager.HideAllPopups();
            StartGameplay();
        }

        public void StartGameplay()
        {
            IsPause = false;
            EndGame = false;
        }

        public void StopGameplay()
        {
            IsPause = true;
            EndGame = true;

            foreach (var item in _controllers)
                item.ResetAll();

            _uIManager.ResetAll();
        }

        public void ChangeAppState(Enumerators.AppState stateTo)
        {
            CurrentState = stateTo;
            switch (stateTo)
            {
                case Enumerators.AppState.AppStart:
                    _uIManager.HideAllPopups();
                    _uIManager.SetPage<MainMenuView>();
                    
                    break;
                case Enumerators.AppState.InGame:
                    _uIManager.HideAllPopups();
                    _uIManager.HideAllPages();

                    GetController<LevelController>().ActivationLevel();

                    GetController<EnemyController>().ActivateAndPositionEnemies();
                    
                    GetController<PlayerController>().ActivationPlayer();
                    GetController<PlayerController>().MovePlayerToStartPosition();

                    GetController<LevelController>().GetStartPositionAndRotationCamera(out var position, out var rotation);
                    GetController<CameraController>().SetPositionAndRotation(position, rotation);
                    GetController<CameraController>().ActivationCameraTrack();
                    StartGameplay();
                    break;
            }
        }
    }
}