using GameSettingUtils;
using PlayerModule.Data;
using System.Threading.Tasks;

namespace GameManagement
{
    public interface IServerGameBackend
    {
        Task PlayerLeftGame(Player p);

        Task ProccessMessage(object message);
        Task Initialize(IGameControl control, IGameMessenger messenger, IGameModeData gameMode, Player[] players);
        Task Start();
        Task Abort();

    }
}