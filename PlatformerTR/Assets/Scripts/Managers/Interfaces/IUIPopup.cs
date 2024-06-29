using System;

namespace Managers.Interfaces
{
    public interface IUIPopup
    {
        void Show();
        void Show(Action callback);
        void Hide();
        void Reset();
    }
}