using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace Sources.Clean.Presentation
{
    public class MetaGameGameStateBoot : IAsyncStartable
    {
        private readonly IScreenManager _screenManager;
        private readonly ScreenId _initialScreenId;

        public MetaGameGameStateBoot(ScreenId initialScreenId, IScreenManager screenManager)
        {
            _initialScreenId = initialScreenId;
            _screenManager = screenManager;
        }
        
        public async void Start()
        {
            // try
            // {
            // }
            // catch (System.Exception e)
            // {
            //     Debug.LogError("Error occured during boot of the Metagame state.");
            //     Debug.LogException(e);
            // }
        }

        public UniTask StartAsync(CancellationToken cancellation = new())
        {
            return _screenManager.ChangeScreen(_initialScreenId);
        }
    }
}