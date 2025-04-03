namespace Sources.Clean.Presentation
{
    public interface IGameStateSceneProvider
    {
        string GetSceneName(GameStateId stateId);
    }
}