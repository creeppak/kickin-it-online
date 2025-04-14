using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Screens;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.Metagame
{
    public class MetaGameScope : AppStateSceneScope
    {
        [SerializeField] private ScreenId initialScreenId;
        [SerializeField] private ScreenPrototypeCollectionAsset screenPrototypes;
        [SerializeField] private ScreenNester screenNester;

        protected override void ConfigureGameStateScope(IContainerBuilder builder)
        {
            builder.RegisterGameScreens(screenPrototypes, screenNester);
            builder.Register<MetaGameBoot>(Lifetime.Singleton);
            builder.RegisterEntryPoint<MetaGameBoot>()
                .WithParameter(initialScreenId);
            
            builder.RegisterEntryPointExceptionHandler(Debug.LogException);
        }
    }
}