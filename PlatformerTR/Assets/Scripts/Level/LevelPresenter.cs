using Core;
using UnityEngine;

namespace Level
{
    public class LevelPresenter : SimplePresenter<LevelPresenter, LevelPresenterView>
    {
        
        public LevelPresenter(LevelPresenterView view) : base(view)
        {
            
        }

        public void GetStartPositionAndRotationPlayer(out Vector3 position, out Quaternion rotation)
        {
            var startTransform = View.GetStartTransformPlayer();

            position = startTransform.position;
            rotation = startTransform.rotation;
        }       
        
        public void GetStartPositionAndRotationCamera(out Vector3 position, out Quaternion rotation)
        {
            var startTransform = View.GetStartCameraTransform();

            position = startTransform.position;
            rotation = startTransform.rotation;
        }
        
        public ConfigurationEnemy[] GetConfigurationEnemy()
        {
            return View.ConfigurationEnemies;
        }
    }
}