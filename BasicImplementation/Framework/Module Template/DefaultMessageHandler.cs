using GameServer;

namespace ServerImplementation
{
    public enum EmptyEnum
    {
    }
    public class DefaultMessageHandler<TData, TLogic> : ModuleMessageHandler<TData, TLogic>
        where TData : IModuleData
        where TLogic : IModuleLogic
    {
        protected override void MessageRegistration()
        {
        }
    }
}
