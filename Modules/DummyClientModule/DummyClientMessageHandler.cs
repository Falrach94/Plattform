using ServerImplementation;

namespace DummyClientModule
{
    public class DummyClientMessageHandler : ModuleMessageHandler<DummyClientData, DummyClientLogic>
    {
        protected override void MessageRegistration()
        {
        }
    }
}