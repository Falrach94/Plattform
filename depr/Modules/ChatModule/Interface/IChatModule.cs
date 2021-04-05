namespace ChatModule
{
    public interface IChatModule : GameServer.IModule
    {
        IChatControl ChatControl { get; }
        IChatStorage ChatStorage { get; }
    }
}