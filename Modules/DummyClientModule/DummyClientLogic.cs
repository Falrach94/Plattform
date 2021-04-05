using DummyClientModule.Client;
using ServerImplementation.Modules;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DummyClientModule
{
    public class DummyClientLogic : ModuleLogic<DummyClientData>
    {
        public Stream LogStream { get; set; }
        public DummyClient CreateClient(string name = null)
        {
            log.Debug("creating new dummy " + name ?? "");
            var client = new DummyClient();

            if (name != null)
            {
                client.Name = name;
            }

            client.LogManager.InstanceName = client.Name;
            if (LogStream != null)
            {
                client.LogManager.Stream = LogStream;
            }

            Data.AddDummy(client);

            return client;
        }
        public void RemoveClient(DummyClient client)
        {
            log.Debug("removing dummy " + client);
            if (client.ConnectionState != ServerClientArchitecture.Connection.ConnectionState.Disconnected)
            {
                client.Disconnect();
            }

            Data.RemoveDummy(client);
        }

        public void ConnectClient(DummyClient client)
        {
            Task.Run(() =>
            {
                log.Debug("connecting dummy " + client);
                _ = client.Connect("localhost", Server.Port);
            });
        }
        public void DisconnectClient(DummyClient client)
        {
            log.Debug("disconnecting dummy " + client);
            client.Disconnect();
        }

        public override Task Reset()
        {
            while(Data.ClientDic.Count != 0)
            {
                RemoveClient(Data.ClientDic.Values.First());
            }
            return Task.CompletedTask;
        }

    }
}