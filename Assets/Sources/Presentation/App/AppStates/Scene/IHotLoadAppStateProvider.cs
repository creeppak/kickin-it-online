namespace KickinIt.Presentation.Game.GameStates
{
    public interface IHotLoadAppStateProvider
    {
        public bool TryRegisterHotLoadState(out AppState appState);
    }
}