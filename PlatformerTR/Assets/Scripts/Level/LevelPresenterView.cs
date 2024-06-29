using System;
using Core;
using Settings;
using UnityEngine;

namespace Level
{
    [PrefabInfo(Enumerators.NamePrefabAddressable.Level)]
    public class LevelPresenterView : SimplePresenterView<LevelPresenter,LevelPresenterView>
    { 
        [SerializeField] private Transform spawnPlacePlayer;
        [SerializeField] private Transform startCameraTransform;

        [SerializeField] private ConfigurationEnemy[] _configurationEnemies;
        public ConfigurationEnemy[] ConfigurationEnemies => _configurationEnemies;

        public override void Init()
        {
            
        }

        public Transform GetStartTransformPlayer() => spawnPlacePlayer;
        public Transform GetStartCameraTransform() => startCameraTransform;
        
    }
    
    [Serializable]
    public class ConfigurationEnemy
    {
        public Enumerators.SpawnEnemy namePrefab;
        public Transform spawnPlace;
        public Transform[] patrolPoints;
        public TriggerZone triggerZone;
    }
}
