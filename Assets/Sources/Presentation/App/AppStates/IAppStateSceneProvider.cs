namespace KickinIt.Presentation.Game.GameStates
{
    public interface IAppStateSceneProvider
    {
        string GetSceneName(AppStateId stateId);
    }
}