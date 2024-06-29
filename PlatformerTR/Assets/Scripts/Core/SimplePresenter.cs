using System;
using Core.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

    namespace Core
    {
        public abstract class SimplePresenter<TP, TV> : IPresenter where TP : SimplePresenter<TP, TV> where TV : SimplePresenterView<TP, TV> {
            protected TV View { get; }
            public bool IsDestroyed { get; private set; }
            public virtual bool IsActive => View.gameObject.activeSelf;
            
            public event Action<IPresenter> Destroyed;

            protected SimplePresenter(TV view) {
                View = view;
                ((IView) View).SetPresenter((TP) this);
                View.Init();
            }
            
            public void SetParent(Transform parent) {
                View.transform.SetParent(parent, false);
            }

            public virtual void SetActive(bool active) {
                View.SetActive(active);
            }

            public virtual void Destroy()
            {
                IsDestroyed = true;
                Destroyed?.Invoke(this);

                if (View != null) {
                    Object.Destroy(View.gameObject);
                }
            }

            public void OnDestroy() {
                if(View!=null)
                ResourceLoader.ReleaseInstance(View.gameObject);
            }
        }
    }
