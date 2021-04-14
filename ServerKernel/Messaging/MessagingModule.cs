using MessageUtils;
using MessageUtils.MessageHandler;
using MessageUtils.Messenger;
using NetworkUtils.Endpoint;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using PatternUtils.Module_Framework.Impl.WeakDependency;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServerKernel.Messaging
{
    public class MessagingModule : ModuleControl
    {
        private IMessageErrorProtocol _errorProtocol;
        private MessageProcessor _messageProcessor;
        private BroadcastMessenger _messenger;
        private ActionBlock<MessageProcessingError> _messageErrorBlock;
        private IDisposable _messageErrorUnsubscriber;
        private readonly Dictionary<IMessageHandler, IDisposable> _handlerDic = new();

        public MessagingModule() : base("Messaging", PatternUtils.Version.Create(1,0,0))
        {
        }

        protected override void Initialize(object[] data)
        {
            _messageErrorBlock = new(HandleMessageError);
        }

        protected override void DefineModule(ModuleBuilder builder)
        {
            _messageProcessor = new();
            _messenger = new(_messageProcessor);

            builder.SetDependencies(new InterfaceInfo(typeof(IMessageErrorProtocol), PatternUtils.Version.Create(1, 0)),
                                    new InterfaceInfo(typeof(IRawMessageReceiver), PatternUtils.Version.Create(1, 0)));
            builder.SetProvidedInterfaces(new ModuleInterfaceWrapper<BroadcastMessenger>(_messenger, PatternUtils.Version.Create(1, 0, 0)));
            builder.SetManagerInterfaces(new ManagerInterface<IMessageHandler>( new(1,0,0), new(1, 0, 0), RegisterNewMessageHandler, UnregisterMessageHandler),
                                         new WeakDependencyProvider<BroadcastMessenger>(_messenger, new(1,0,0)));
           

        }

        private async Task HandleMessageError(MessageProcessingError error)
        {
            if (_errorProtocol != null)
            {
                await _errorProtocol.HandleMessageErrorAsync(error);
            }
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
            _errorProtocol = await interfaceProvider.GetInterfaceAsync<IMessageErrorProtocol>(token);
            var messageReceiver = await interfaceProvider.GetInterfaceAsync<IRawMessageReceiver>(token);
            _messageProcessor.SetMessageReceiver(messageReceiver);
            _messageErrorUnsubscriber = _messageProcessor.ErrorBlock.LinkTo(_messageErrorBlock);


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
            _errorProtocol = null;
            _messageProcessor.SetMessageReceiver(null);
            _messageErrorUnsubscriber?.Dispose();
            _messageErrorUnsubscriber = null;
            return Task.CompletedTask;
        }
    }
}