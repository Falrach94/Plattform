using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using ServerKernel.Network;
using ServerKernel.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncUtils;
using ServerKernel.Connections.Manager;

namespace ServerKernel.Control
{
    public class ControlModule : ModuleControl, IServerControl
    {
        private readonly IModuleManager _moduleManager;
        private readonly ConnectionProtocol _procotol = new();

        private INetworkControl _network;

        public ControlModule(IModuleManager manager) : base("Control", PatternUtils.Version.Create(1,0,0))
        {
            _moduleManager = manager;
        }

        public async Task ResetAllAsync(int port)
        {
            _network.TargetPort = port;
            await _moduleManager.StopAllModulesAsync();
            await _moduleManager.ResetAllModulesAsync();
            await _moduleManager.StartAllModulesAsync();
        }
        public async Task ResetNetworkAsync(int port)
        {
            _network.TargetPort = port;
            await _moduleManager.StopModuleAsync(Header, true);
            await _moduleManager.ResetModuleAsync(Header);
            await _moduleManager.StartModuleAsync(Header, true);
        }

        protected override void DefineModule(ModuleBuilder builder)
        {
            builder.SetProvidedInterfaces(new ModuleInterfaceWrapper<IConnectionProtocolHandler>(_procotol, PatternUtils.Version.Create(1, 0, 0)),
                                          new ModuleInterfaceWrapper<IServerControl>(this, PatternUtils.Version.Create(1, 0, 0)));

            builder.SetDependencies(new InterfaceInfo(typeof(INetworkControl), PatternUtils.Version.Create(1,0)));
        }

        protected override async Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken token)
        {
            _network = await interfaceProvider.GetInterfaceAsync<INetworkControl>(token);
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
            _network = null;
            return Task.CompletedTask;
        }
    }
}
