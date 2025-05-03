using KickinIt.Presentation.Match;
using KickinIt.Simulation;
using Sources.Common;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class HudScreenInstaller : MonoInstaller
{
    public override void Install(IContainerBuilder builder)
    {
        Debug.Log("HUDScreenInstaller install");
        
        builder.RegisterFactory<IPlayer, HudPlayerPresenter, RectTransform, HudPlayerPresenter>(
            resolver =>
                (player, prefab, container) => BuildPlayerHUD(resolver, player, prefab, container),
            Lifetime.Singleton);
    }

    private static HudPlayerPresenter BuildPlayerHUD(IObjectResolver resolver, IPlayer player, HudPlayerPresenter prefab,
        RectTransform container)
    {
        var playerHudScope = resolver.CreateScope(
            builder =>
            {
                builder.RegisterInstance(player).As<IPlayer>();
            });
        return playerHudScope.Instantiate(prefab, container, worldPositionStays: false);
    }
}