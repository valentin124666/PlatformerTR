using System;
using Managers.Interfaces;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuView : MonoBehaviour,IUIElement
    {
        [SerializeField] private Button startButton;

        private void Awake()
        {
            startButton.onClick.AddListener(()=>GameClient.Get<IGameplayManager>().ChangeAppState(Enumerators.AppState.InGame));
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            startButton.onClick.RemoveAllListeners();
        }
    }
}
