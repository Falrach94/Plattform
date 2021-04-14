using MessageUtils.MessageHandler;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using PatternUtils.Module_Framework.Impl.WeakDependency;
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

     //   private IConnectionProtocolHandler _protocol;
        //private IEndpointObservable _endpoints;

        public ConnectionModule() : base("Connections", PatternUtils.Version.Create(1,0,0))
        {
        }

        protected override void DefineModule(ModuleBuilder builder)
        {
            builder.SetDependencies(new InterfaceInfo(typeof(IConnectionProtocolHandler), PatternUtils.Version.Create(1, 0)),
                                    new InterfaceInfo(typeof(IEndpointControl), PatternUtils.Version.Create(1, 0)),
                                    new InterfaceInfo(typeof(IEndpointObservable), PatternUtils.Version.Create(1, 0)));
            builder.SetProvidedInterfaces(new ModuleInterfaceWrapper<IConnectionControl>(_connectionManager, PatternUtils.Version.Create(1, 0, 0)));
            builder.SetManagerInterfaces(new ManagerInterface<IConnectionProcessor>(new(1, 0, 0), new(1, 0, 0), RegisterProcessor, UnregisterProcessor),
                                         new WeakDependencyProvider<IConnectionControl>(_connectionManager, new(1, 0, 0)));
        }

        private void RegisterProcessor(IConnectionProcessor processor)
        {
            _processorDic.Add(processor, _connectionManager.RegisterConnectionProcessor(processor));
        }
        private void UnregisterProcessor(IConnectionProcessor processor)
        {
            _processorDic[processor].Dispose();
        }


        protected override async Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken token)
        {
            _connectionManager.Endpoints = await interfaceProvider.GetInterfaceAsync<IEndpointObservable>(token); ;

            _connectionManager.EndpointControl = await interfaceProvider.GetInterfaceAsync<IEndpointControl>(token); ;

            _connectionManager.ProtocolHandler = await interfaceProvider.GetInterfaceAsync<IConnectionProtocolHandler>(token);
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
            _connectionManager.Endpoints = null;
            _connectionManager.EndpointControl = null;
            _connectionManager.ProtocolHandler = null;

            return Task.CompletedTask;
        }
    }
}
