using DiagnosticsModule;
using DiagnosticsModule.Interface;
using DummyClientModule.Client;
using ServerImplementation.Modules;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DummyClientModule
{
    public class DummyClientData : ModuleData, IDiagnosticsDataInterface
    {
        public IReadOnlyDictionary<int, DummyClient> ClientDic => _clientDic;

        private readonly Dictionary<int, DummyClient> _clientDic = new Dictionary<int, DummyClient>();

        public void AddDummy(DummyClient dummy)
        {
            _clientDic.Add(dummy.Id, dummy);
            log.Debug("dummy " + dummy + " added");
        }
        public void RemoveDummy(DummyClient dummy)
        {
            _clientDic.Remove(dummy.Id);
            log.Debug("dummy " + dummy + " removed");
        }

        public DummyClient GetClient(string name)
        {
            var client = ClientDic.Values.Where(c => c.Name == name);
            if (!client.Any())
            {
                return null;
            }
            return client.First();
        }

        public string GetDetailedState()
        {
            return DiagnosticsUtils.ListString("Dummies", _clientDic);
        }
    }
}