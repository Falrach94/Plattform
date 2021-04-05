using ControlModule;
using ControlModule.Logic;
using GameServer;
using ServerImplementation;
using ServerImplementation.Modules;
using TextInterfaceModule.Interface;

namespace TextInterfaceModule
{
    public class TextInterfaceModule : Module<TextInterfaceLogic, TextInterfaceData, DefaultMessageHandler<TextInterfaceData, TextInterfaceLogic>>, ITextInterfaceModule
    {
        public ICommandInterface CommandInterface => Logic;
        public ITextInterfaceControl InterfaceControl => Logic;

        public TextInterfaceModule(IServerControl control, IServerInfo info)
            : base("Text Interface", "Text Interface")
        {
            Data.Control = control;
            Data.Info = info;
        }


        public override void GreetModule(IModule module)
        {
            if (module is IRegisterTextInterface ruleSet)
            {
                ruleSet.RegisterTextInterface(Logic);
            }
        }
    }
}