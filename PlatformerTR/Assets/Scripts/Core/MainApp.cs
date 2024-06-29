using System;
using Cysharp.Threading.Tasks;
using Managers.Interfaces;
using Settings;
using UnityEngine;

namespace Core
{
    public class MainApp : MonoBehaviour
    {
        public event Action LateUpdateEvent;
        public event Action FixedUpdateEvent;
        public event Action UpdateEvent;

        private static MainApp _Instance;

        public static MainApp Instance
        {
            get { return _Instance; }
            private set { _Instance = value; }
        }


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (Instance == this)
            {
                ResourceLoading().Forget();
            }
        }

        private async UniTask ResourceLoading()
        {
            await ResourceLoader.Init();

            await GameClient.Instance.InitServices();

            GameClient.Get<IGameplayManager>().ChangeAppState(Enumerators.AppState.AppStart);
        }

        void Update()
        {
            if (Instance == this)
            {
                UpdateEvent?.Invoke();
            }
        }

        private void LateUpdate()
        {
            if (Instance == this)
            {
                LateUpdateEvent?.Invoke();
            }
        }

        private void FixedUpdate()
        {
            if (Instance == this)
            {
                FixedUpdateEvent?.Invoke();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                GameClient.Instance.Dispose();
            }
        }
    }
}