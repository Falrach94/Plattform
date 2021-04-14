using MessageUtils.Messenger;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using PatternUtils.Module_Framework.Impl.WeakDependency;
using ServerKernel.Connections.Manager;
using ServerKernel.Messaging;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKernel.Protocol
{
    public class ProtocolModule : ModuleControl
    {
        private MessageProtocol _messageErrorProtocol = new();
        private ConnectionProtocol _connectionProtocol = new();

        private WeakDependency<BroadcastMessenger> _messengerDependency;
        private WeakDependency<IConnectionControl> _connectionControlDependency;

        public ProtocolModule() : base("Protocol", PatternUtils.Version.Create(1))
        {
        }

        protected override void Initialize(object[] data)
        {
            _messengerDependency = new(new(1, 0, 0), SetMessenger);
            _connectionControlDependency = new(new(1, 0, 0), SetConnectionControl);
        }

        private void SetMessenger(WeakDependency<BroadcastMessenger> _, BroadcastMessenger messenger)
        {
            _messageErrorProtocol.Messenger = messenger;
            _connectionProtocol.Messenger = messenger;
        }
        private void SetConnectionControl(WeakDependency<IConnectionControl> _, IConnectionControl control)
        {
            _messageErrorProtocol.ConnectionControl = control;
            _connectionProtocol.ConnectionControl = control;
        }


        protected override void DefineModule(ModuleBuilder builder)
        {
            builder.SetProvidedInterfaces(new ModuleInterfaceWrapper<IMessageErrorProtocol>(_messageErrorProtocol, PatternUtils.Version.Create(1)),
                                          new ModuleInterfaceWrapper<IConnectionProtocolHandler>(_connectionProtocol, PatternUtils.Version.Create(1)));

            builder.SetManagedInterfaces(_messengerDependency.CreateWrapper(),
                                         _connectionControlDependency.CreateWrapper());
        }

        protected override Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken providerLockToken)
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
