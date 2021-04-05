using DiagnosticsModule.Interface;
using ServerImplementation.Modules;
using System.Collections.Generic;

namespace DiagnosticsModule
{
    public class DiagnosticsData : ModuleData, IDiagnosticsDataInterface
    {
        private readonly Dictionary<string, IDiagnosticsDataInterface> _moduleDic = new Dictionary<string, IDiagnosticsDataInterface>();

        public IReadOnlyDictionary<string, IDiagnosticsDataInterface> ModuleDic => _moduleDic;

        public void AddModule(string name, IDiagnosticsDataInterface module)
        {
            _moduleDic.Add(name, module);
            log.Debug("added diagnostics of module " + name);
        }

        public string GetDetailedState()
        {
            string output = "registered modules:\n   ";
            output += string.Join("\n   ", _moduleDic.Keys);
            return output;
        }

    }
}