using DiagnosticsModule.Interface;
using ServerImplementation.Modules;
using System;
using System.Threading.Tasks;

namespace DiagnosticsModule
{
    public class DiagnosticsLogic : ModuleLogic<DiagnosticsData>, IDiagnosticsControl
    {
        public IDiagnosticsDataInterface Diagnostics => Data;

        public void RegisterModule(string name, Interface.IDiagnosticsDataInterface diagnostics)
        {
            Data.AddModule(name.ToUpper(), diagnostics);
        }

        public string GetDetailedState(string module)
        {
            module = module.ToUpper();
            if (!Data.ModuleDic.ContainsKey(module))
            {
                throw new ArgumentException("No module with this name registered!");
            }
            return Data.ModuleDic[module].GetDetailedState();
        }

        public override Task Reset() 
        {
            return Task.CompletedTask;
        }
    }
}