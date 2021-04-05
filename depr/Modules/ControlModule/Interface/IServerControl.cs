namespace ControlModule
{
    public interface IServerControl
    {
        void CloseServer();
        void Kick(int id, string msg);
        void SetAccessLevel(int id, ServerRightsLevel level);


    }
}