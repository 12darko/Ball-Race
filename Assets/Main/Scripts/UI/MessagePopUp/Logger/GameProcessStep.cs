namespace _Main.Scripts.UI.MessagePopUp.Logger
{
    public enum GameProcessStep
    {
        None,
        CreatingLobby,
        LobbyCreated,
        JoiningLobby,
        LobbyJoined,
        RelayAllocating,
        RelayConnected,
        ConnectingToServer,
        ConnectedToServer,
        InLobby,
        Error,
        Complete,
        Disconnecting,
        Disconnected
    }
}