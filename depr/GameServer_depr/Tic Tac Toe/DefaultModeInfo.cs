using GameManagement;
using GameSettingUtils;
using GameUtilities.GameMode;
using GameUtilities.ServerBackend;

namespace TicTacToe
{
    public class DefaultModeInfo : GameModeInfo<DefaultMode>, IServerGameMode
    {
        public DefaultModeInfo() 
            : base("default", 2, 2)
        {
        }

        public IGameModel Model => throw new System.NotImplementedException();

        public IServerGameBackend Backend => throw new System.NotImplementedException();

        protected override void RegisterSettings(ISettingsBuilder builder)
        {
            builder.AddIntegerSetting("size", 3, 5);
        }
    }
}
