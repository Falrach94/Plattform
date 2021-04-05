using DiagnosticsModule.Interface;
using GameServer;
using ServerImplementation.Modules;
using TextInterfaceModule.Interface;
using TextInterfaceModule.Logic.Parser;
using TextParser.Parser;

namespace DiagnosticsModule
{
    public class DiagnosticsModule : Module<DiagnosticsLogic, DiagnosticsData, DiagnosticsMessageHandler>, IRegisterTextInterface, IDiagnosticsControl, IDiagnosticsManager
    {
        public IDiagnosticsDataInterface Diagnostics => Data;

        public DiagnosticsModule() : base("Diagnostics", "Diagnostics Module")
        {
            Logic.RegisterModule(this.Name, Diagnostics);
        }

        public void RegisterTextInterface(ITextInterfaceControl textInterface)
        {
            textInterface.AddToken("State", "^state");

            textInterface.AddRule(new CommandParserRule(
                ParserSequence.Start().Token("State").String("Module").End(),
                (c) =>
                {
                    string module = c.Get<string>("Module");
                    string output = "state of module " + module + ": \n";
                    output += "--------------------------------------\n";
                    output += Logic.GetDetailedState(module) + "\n";
                    output += "--------------------------------------\n";
                    return output;
                }));
        }

        public override void GreetModule(IModule module)
        {
            if (module is IDiagnosticsControl diagnosticModule)
            {
                Logic.RegisterModule(module.Name, diagnosticModule.Diagnostics);
            }
        }

        public string GetSystemState()
        {
            string output = "";

            foreach (var m in Data.ModuleDic.Keys)
            {
                output += "state of module " + m + ": \n";
                output += "--------------------------------------\n";
                output += Logic.GetDetailedState(m) + "\n";
                output += "--------------------------------------\n";
                // output += m + "\n";
                // output += Logic.GetDetailedState(m)+"\n";
            }

            return output;
        }
    }
}
