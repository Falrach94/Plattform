using NetworkUtils.Endpoint;
using NetworkUtils.Socket;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using ServerKernel.Connections.Manager;
using ServerKernel.Messaging;
using ServerUtils;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServerTests.Mocks
{

    public class MockBaseModule : ModuleControl, IEndpointObservable, IEndpointControl, IRawMessageReceiver
    {
        public MockProtocol MockProtocol = new();


        public MockBaseModule() : base("Network", PatternUtils.Version.Create(1))
        {
        }

        public List<IEndpoint> ConnectedEndpoints { get; } = new();

        public EndpointConnectedCallback EndpointConnectedHandler { get; set; }
        public EndpointDisconnectedCallback EndpointDisconnectedHandler { get; set; }

        private BufferBlock<RawMessage> _rawMsgBlock = new();

        public ISourceBlock<RawMessage> RawMessageBlock => _rawMsgBlock;

        SemaphoreSlim _sem = new(1, 1);

        private Dictionary<IEndpoint, IDisposable> _disposeDic = new();

        public void SendMessage(object sender, byte[] data)
        {
            _rawMsgBlock.Post(new RawMessage(sender, data));
        }

        public void ConnectEndpoint(IEndpoint ep)
        {
            _sem.Wait();
            ConnectedEndpoints.Add(ep);
            _disposeDic.Add(ep, ep.RawMessageBlock.LinkTo(_rawMsgBlock));
            _sem.Release();


            ep.Disconnected += Ep_Disconnected;
            EndpointConnectedHandler?.Invoke(ep);


        }

        private void Ep_Disconnected(object sender, DisconnectedArgs e)
        {
            IEndpoint ep = (IEndpoint)sender;
            _sem.Wait();
            ConnectedEndpoints.Remove(ep);
            _disposeDic[ep].Dispose();
            _disposeDic.Remove(ep);
            _sem.Release();

            EndpointDisconnectedHandler?.Invoke(ep, e.RemoteDisconnect);
        }

        public Task DisconnectAllAsync()
        {
            var copy = new List<IEndpoint>(ConnectedEndpoints);
            foreach (var ep in copy)
            {
                DisconnectEndpointAsync(ep);
            }
            return Task.CompletedTask;
        }

        public void DisconnectEndpoint(IEndpoint ep, bool remote)
        {
            ((MockSocket)ep.Socket).Disconnect(remote);
            // EndpointDisconnectedHandler?.Invoke(ep, remote);
        }

        public Task DisconnectEndpointAsync(IEndpoint endpoint)
        {
            DisconnectEndpoint(endpoint, false);
            return Task.CompletedTask;
        }

        protected override void DefineModule(ModuleBuilder builder)
        {
            builder.SetProvidedInterfaces(new ModuleInterfaceWrapper<IEndpointObservable>(this, new(1, 0, 0)),
                                            new ModuleInterfaceWrapper<IEndpointControl>(this, new(1, 0, 0)),
                                            new ModuleInterfaceWrapper<IRawMessageReceiver>(this, new(1, 0, 0)),
                                            new ModuleInterfaceWrapper<IConnectionProtocolHandler>(MockProtocol, new(1, 0, 0)),
                                            new ModuleInterfaceWrapper<IMessageErrorProtocol>(MockProtocol, new(1, 0, 0)));
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
