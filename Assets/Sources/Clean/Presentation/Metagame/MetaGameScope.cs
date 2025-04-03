using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sources.Clean.Presentation
{
    public class MetaGameScope : GameStateSceneScope
    {
        [SerializeField] private ScreenId initialScreenId;
        [SerializeField] private ScreenPrototypeCollectionAsset screenPrototypes;
        [SerializeField] private ScreenNester screenNester;

        protected override void ConfigureGameStateScope(IContainerBuilder builder)
        {
            builder.RegisterGameScreens(screenPrototypes, screenNester);
            builder.Register<MetaGameGameStateBoot>(Lifetime.Singleton);
            builder.RegisterEntryPoint<MetaGameGameStateBoot>()
                .WithParameter(initialScreenId);
            
            builder.RegisterEntryPointExceptionHandler(Debug.LogException);
        }
    }
}