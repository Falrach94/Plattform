using GameSettingUtils;

namespace GameManagement
{
    public interface IServerGameFactory
    {
        string Name { get; }

        ISettingsProvider CreateLobbySettingsProvider();
        IServerGameBackend CreateGameInstance(string mode);
    }
}