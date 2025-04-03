using System;
using UnityEngine;
using VContainer.Unity;

namespace Sources.Clean.Presentation
{
    public abstract class GameScreenPresenter : MonoBehaviour, IStartable, IDisposable
    {
        protected virtual void OnScreenLoaded()
        {
            // empty for base class
        }

        protected virtual void OnScreenDispose()
        {
            // empty for base class
        }

        void IStartable.Start()
        {
            OnScreenLoaded();
        }

        void IDisposable.Dispose()
        {
            OnScreenDispose();
        }
    }
}