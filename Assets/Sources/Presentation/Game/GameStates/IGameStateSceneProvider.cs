namespace KickinIt.Presentation.Game.GameStates
{
    public interface IGameStateSceneProvider
    {
        string GetSceneName(GameStateId stateId);
    }
}