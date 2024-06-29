using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Interfaces;
using Cysharp.Threading.Tasks;
using Enemy;
using Level;
using Managers.Interfaces;
using UnityEngine;

namespace Managers.Controllers
{
    public class EnemyController : IController
    {
        private Transform _poolEnemy;
        private LevelController _levelController;
        private ConfigurationEnemy[] _config;

        private List<EnemyPresenter> _enemyPresenters;

        public bool IsInit { get; private set; }

        public void Init()
        {
            _poolEnemy = new GameObject("[PoolEnemy]").transform;
            _levelController = GameClient.Get<IGameplayManager>().GetController<LevelController>();
            CreatePlayerPresenters().Forget();
        }

        private async UniTask CreatePlayerPresenters()
        {
            await UniTask.WaitUntil(() => _levelController.IsInit);
            _config = _levelController.GetConfigurationEnemy().ToArray();
            var presenters = await ResourceLoader.InstantiateMultiple<EnemyPresenter, EnemyPresenterView>(_poolEnemy, "", _config.Length);

            _enemyPresenters = presenters.ToList();

            for (int i = 0; i < _enemyPresenters.Count; i++)
            {

                _enemyPresenters[i].SetActive(false);
            }

            IsInit = true;
        }

        private void RemoveEnemyPresenters(IPresenter presenter)
        {
            if (presenter is not EnemyPresenter enemyPresenter || _enemyPresenters == null || !_enemyPresenters.Contains(presenter))
            {
                return;
            }

            _enemyPresenters.Remove(enemyPresenter);
        }

        public void ActivateAndPositionEnemies()
        {
            for (int i = 0; i < _enemyPresenters.Count; i++)
            {
                _enemyPresenters[i].SetActive(true);

                var transformSpawn = _config[i].spawnPlace;
                _enemyPresenters[i].SetPositionAndRotation(transformSpawn.position, transformSpawn.rotation);

                var point = _config[i].patrolPoints.Select(tran => tran.position);
                _enemyPresenters[i].SetPatrol(_config[i].triggerZone, point);
                _enemyPresenters[i].Destroyed += RemoveEnemyPresenters;
            }
        }
        
        public void Dispose()
        {
            var presenters = _enemyPresenters;
            _enemyPresenters = null;
            for (int i = 0; i < presenters.Count; i++)
            {
                presenters[i]?.Destroy();
            }
        }

        public void ResetAll()
        {
        }
    }
}