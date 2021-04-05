using MessageUtils;
using MessageUtils.MessageHandler;
using MessageUtils.Messenger;
using NetworkUtils.Endpoint;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKernel.Messaging
{
    public class MessagingModule : ModuleControl
    {
        private readonly MessageProcessor _messageProcessor;
        private readonly BroadcastMessenger _messenger;

        private readonly Dictionary<IMessageHandler, IDisposable> _handlerDic = new();

        public MessagingModule() : base("Messaging", PatternUtils.Version.Create(1,0,0))
        {
            _messageProcessor = new();
            _messenger = new(_messageProcessor);
        }

        protected override void DefineModule(ModuleBuilder builder)
        {
            builder.SetDependencies(new InterfaceInfo(typeof(IRawMessageReceiver), PatternUtils.Version.Create(1, 0)));
            builder.SetProvidedInterfaces(new ModuleInterfaceWrapper<IBroadcastMessenger>(_messenger, PatternUtils.Version.Create(1, 0, 0)));
            builder.SetManagerInterfaces(new ManagerInterface<IMessageHandler>( PatternUtils.Version.Create(1, 0, 0), 
                                                                                PatternUtils.Version.Create(1, 0, 0),
                                                                                RegisterNewMessageHandler, UnregisterMessageHandler));
        }

        private void UnregisterMessageHandler(IMessageHandler handler)
        {
            _handlerDic[handler].Dispose();
            _handlerDic.Remove(handler);
        }

        private void RegisterNewMessageHandler(IMessageHandler handler)
        {
            _handlerDic.Add(handler, _messageProcessor.RegisterMessageHandler(handler));
            
        }

        protected override async Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken token)
        {
            var messageReceiver = await interfaceProvider.GetInterfaceAsync<IRawMessageReceiver>(token);
            _messageProcessor.SetMessageReceiver(messageReceiver);
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
            _messageProcessor.SetMessageReceiver(null);
            return Task.CompletedTask;
        }
    }
}