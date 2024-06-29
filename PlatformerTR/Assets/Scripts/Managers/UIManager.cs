using System.Collections.Generic;
using Core;
using Core.Interfaces;
using Cysharp.Threading.Tasks;
using Managers.Interfaces;
using Settings;
using UI;
using UnityEngine;

namespace Managers
{
    public class UIManager : IService, IUIManager
    {
        private List<IUIElement> _uiPages;
        private List<IUIPopup> _uiPopups;

        private GameObject _canvas { get; set; }

        public IUIElement CurentPage { get; set; }

        public async UniTask Init()
        {
            // Зроби щось нормальне ліз 
            _canvas = Object.FindObjectOfType<Canvas>().gameObject;

            _uiPages = new List<IUIElement>();

            _uiPopups = new List<IUIPopup>();
            await CreateUI();
        }

        private async UniTask CreateUI()
        {
            _uiPages.Add(await ResourceLoader.Instantiate<MainMenuView>(Enumerators.NamePrefabAddressable.MainMenu.ToString(), _canvas.transform));

        }
        public void ResetAll()
        {
            foreach (var page in _uiPages)
                page.Reset();

            foreach (var popup in _uiPopups)
                popup.Reset();

        }
        public void Dispose()
        {
        }

        public T GetPage<T>() where T : IUIElement
        {
            IUIElement page = null;
            foreach (var _page in _uiPages)
            {
                if (_page is T)
                {
                    page = _page;
                    break;
                }
            }
            return (T)page;
        }
        public T GetPopup<T>() where T : IUIPopup
        {
            IUIPopup popup = null;
            foreach (var _popup in _uiPopups)
            {
                if (_popup is T)
                {
                    popup = _popup;
                    break;
                }
            }

            return (T)popup;
        }
        public void SetPage<T>(bool hideAll = false) where T : IUIElement
        {
            if (hideAll)
            {
                HideAllPages();
            }
            else
            {
                if (CurentPage != null)
                    CurentPage.Hide();
            }

            foreach (var _page in _uiPages)
            {
                if (_page is T)
                {
                    CurentPage = _page;
                    break;
                }
            }

            CurentPage.Show();
        }
        public void DrawPopup<T>() where T : IUIPopup
        {
            IUIPopup popup = null;
            foreach (var _popup in _uiPopups)
            {
                if (_popup is T)
                {
                    popup = _popup;
                    break;
                }
            }
            popup.Show();
        }
        public void HidePopup<T>() where T : IUIPopup
        {
            foreach (var _popup in _uiPopups)
            {
                if (_popup is T)
                {
                    _popup.Hide();
                    break;
                }
            }
        }
        public void HideAllPages()
        {
            foreach (var _page in _uiPages)
            {
                _page.Hide();
            }
        }
        public void HideAllPopups()
        {
            foreach (var _popup in _uiPopups)
            {
                _popup.Hide();
            }
        }

    }
}
