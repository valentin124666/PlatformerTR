using Core;
using Core.Interfaces;
using Cysharp.Threading.Tasks;
using Managers.Interfaces;

namespace Managers
{
    public class DataManager : IService, IDataManager
    {
        private DataConfig _dataConfig;
        
        public async UniTask Init()
        {
            _dataConfig = await ResourceLoader.GetResource<DataConfig>("DataConfig");
        }

        public void Dispose()
        {
            
        }

        public PlayerData GetPlayerData() => _dataConfig.GetPlayerData();

        public EnemyData GetEnemyData()=> _dataConfig.GetEnemyData();

    }
}
