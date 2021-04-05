using NetworkUtils.Endpoint;
using NetworkUtils.Tcp_Client_Listener;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using PatternUtils.Module_Framework.Interfaces;
using ServerKernel.Network;
using ServerUtils;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServerKernel.Network
{
    public class NetworkModule : ModuleControl, INetwork
    {
        const int DEFAULT_PORT = 6666;

        private TcpClientListener _listener = new ();

        private EndpointManager _endpointManager = new();

        public NetworkModule() : base("Network", PatternUtils.Version.Create(1,0,0))
        {
        }

        public int TargetPort { get; set; }

        public int Port { get; } = DEFAULT_PORT;

        protected override void DefineModule(ModuleBuilder builder)
        {
            builder.SetProvidedInterfaces(new ModuleInterfaceWrapper<IRawMessageReceiver>(_endpointManager, PatternUtils.Version.Create(1, 0, 0)),
                                          new ModuleInterfaceWrapper<IEndpointManager>(_endpointManager, PatternUtils.Version.Create(1, 0, 0)),
                                          new ModuleInterfaceWrapper<INetwork>(this, PatternUtils.Version.Create(1, 0, 0)));
        }

        protected override Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken providerLockToken)
        {
            return Task.CompletedTask;
        }

        protected async override Task ResetAsync()
        {
            await _endpointManager.DisconnectAllAsync();
        }

        protected override Task StartAsync()
        {
            _listener.StartListeningForConnections(TargetPort);
            return Task.CompletedTask;
        }

        protected async override Task StopAsync()
        {
            await _listener.StopListeningForConnectionsAsync();
        }

        protected override Task UninitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
