using GameServer.Data_Objects;

namespace GameServer
{
    public interface IModuleData
    {
        bool IsSynchronized(IConnection con);
        void Load();
        void Save();
    }
}