using MessageUtils.MessageHandler;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using ServerKernel.Connections.Manager;
using ServerKernel.Control;
using ServerUtils;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKernel.Connections
{
    public class ConnectionModule : ModuleControl
    {
        private readonly Dictionary<IConnectionProcessor, IDisposable> _processorDic = new();
        private readonly ConnectionManager _connectionManager = new();

        public ConnectionModule() : base("Connections", PatternUtils.Version.Create(1,0,0))
        {
        }

        protected override void DefineModule(ModuleBuilder builder)
        {
            builder.SetDependencies(new InterfaceInfo(typeof(ConnectionProtocol), PatternUtils.Version.Create(1, 0)),
                                    new InterfaceInfo(typeof(IEndpointManager), PatternUtils.Version.Create(1, 0)));
            builder.SetProvidedInterfaces(new ModuleInterfaceWrapper<IConnectionManager>(_connectionManager, PatternUtils.Version.Create(1, 0, 0)));
            builder.SetManagerInterfaces(new ManagerInterface<IConnectionProcessor>(PatternUtils.Version.Create(1, 0, 0),
                                                                                PatternUtils.Version.Create(1, 0, 0),
                                                                                RegisterProcessor, UnregisterProcessor));
        }

        private void RegisterProcessor(IConnectionProcessor processor)
        {
            _processorDic[processor].Dispose();
        }
        private void UnregisterProcessor(IConnectionProcessor processor)
        {
            _processorDic.Add(processor, _connectionManager.RegisterConnectionProcessor(processor));
        }


        protected override Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken token)
        {
            return Task.CompletedTask;
        }

        protected override Task ResetAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task StartAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task StopAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task UninitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
