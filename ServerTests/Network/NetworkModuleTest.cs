using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtils.Endpoint;
using NetworkUtils.Socket;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Data.builder;
using SererTests.TestUtilities;
using ServerKernel.Network;
using ServerUtils;
using ServerUtils.Endpoint_Manager;
using SyncUtils;
using SyncUtils.Barrier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerTests.Network
{
    [TestClass]
    public class NetworkModuleTest
    {
        class NetworkConsumerMockModule : ModuleControl
        {
            public IRawMessageReceiver RawMessageReceiver { get; private set; }
            public IEndpointControl EndpointControl { get; private set; }
            public IEndpointObservable EndpointObservable { get; private set; }
            public INetwork Network { get; private set; }

            public NetworkConsumerMockModule() : base("Mock", PatternUtils.Version.Create(1))
            {
            }

            protected override void DefineModule(ModuleBuilder builder)
            {
                builder.SetDependencies(new InterfaceInfo(typeof(IRawMessageReceiver), PatternUtils.Version.Create(1)),
                                        new InterfaceInfo(typeof(IEndpointControl), PatternUtils.Version.Create(1)),
                                        new InterfaceInfo(typeof(INetwork), PatternUtils.Version.Create(1)),
                                        new InterfaceInfo(typeof(IEndpointObservable), PatternUtils.Version.Create(1)));
            }

            protected override async Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken token)
            {
                RawMessageReceiver = await interfaceProvider.GetInterfaceAsync<IRawMessageReceiver>(token);
                EndpointControl = await interfaceProvider.GetInterfaceAsync<IEndpointControl>(token);
                EndpointObservable = await interfaceProvider.GetInterfaceAsync<IEndpointObservable>(token);
                Network = await interfaceProvider.GetInterfaceAsync<INetwork>(token);
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

        private Task<PacketSocket> ConnectSocketAsync(int port)
        {
            return Task.Run(() =>
            {
                TcpClient client = new TcpClient("localhost", port);
                return new PacketSocket(client);
            });
        }

        [TestMethod]
        public void TestBasicFunctionality()
        {
            const int INIT_PORT = 1233;
            const int PORT = 2342;
            const int PARALLEL_CONNECTS = 20;

            ModuleManager manager = new();

            NetworkModule networkModule = new();
            NetworkConsumerMockModule mockModule = new();

            AsyncBarrier barrier = new(PARALLEL_CONNECTS, false);

            //
            // register modules and ensure all interfaces are present and recognized
            // 
            // network: (1)
            //  start
            //  stop 
            //  start on different port
            //  connect (multiple connects)
            //
            // epControl
            //  disconnect (4)
            //  disconnect all (6)
            //
            // epObservable => EndpointManager tests
            //
            // msgReceiver
            //

            // register modules and ensure all interfaces are present and recognized
            TestUtils.AssertTask(manager.RegisterModuleAsync(networkModule.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(mockModule.Header));

            var msgReceiver = TestUtils.AssertTask(manager.GetInterfaceAsync<IRawMessageReceiver>(null));
            var epControl = TestUtils.AssertTask(manager.GetInterfaceAsync<IEndpointControl>(null));
            var epObservable = TestUtils.AssertTask(manager.GetInterfaceAsync<IEndpointObservable>(null));
            var network = TestUtils.AssertTask(manager.GetInterfaceAsync<INetwork>(null));


            Assert.IsNotNull(msgReceiver);
            Assert.IsNotNull(epControl);
            Assert.IsNotNull(epObservable);
            Assert.IsNotNull(network);

            Assert.AreEqual(msgReceiver, mockModule.RawMessageReceiver);
            Assert.AreEqual(epControl, mockModule.EndpointControl);
            Assert.AreEqual(epObservable, mockModule.EndpointObservable);
            Assert.AreEqual(network, mockModule.Network);



            //
            // network:
            //

            // start
            network.TargetPort = INIT_PORT;
            TestUtils.AssertTask(manager.StartAllModulesAsync());
            Assert.AreEqual(INIT_PORT, network.Port);

            // stop 
            TestUtils.AssertTask(manager.StopAllModulesAsync());

            // start on different port
            network.TargetPort = PORT;
            TestUtils.AssertTask(manager.StartAllModulesAsync());
            Assert.AreEqual(PORT, network.Port);

            //connect (multiple connects)
            SemaphoreSlim sem = new(1,1);
            TaskCompletionSource tcs = new();
            bool exThrown = false;

            IEndpoint exceptionEndpoint = null;
            List<IEndpoint> endpoints = new();

            epObservable.EndpointConnectedHandler = async ep =>
            {
                await sem.WaitAsync();
                try
                {
                    if (!exThrown)
                    {
                        exceptionEndpoint = ep;
                        exThrown = true;
                        tcs.SetResult();
                        throw new Exception();
                    }
                    endpoints.Add(ep);
                }
                finally
                {
                    sem.Release();
                    await Task.Delay(300);
                    TestUtils.AssertTask(barrier.SignalAsync(), 10000);
                }

            };

            bool disconnectDone = false;

            epObservable.EndpointDisconnectedHandler = async (_,_) =>
            {
                await sem.WaitAsync();
                if(disconnectDone)
                {
                    Assert.IsTrue(false, "unexpected disconnect");
                }
                disconnectDone = true;
                sem.Release();
            };
            List<Task<PacketSocket>> tasks = new();
            for(int i = 0; i < PARALLEL_CONNECTS; i++)
            {
                tasks.Add(ConnectSocketAsync(PORT));
            }
            TestUtils.AssertTask(Task.WhenAll(tasks), 10000);

            List<ISocket> sockets = endpoints.Select(ep => ep.Socket).ToList();

            foreach(var s in sockets)
            {
                Assert.IsTrue(s.IsReceiving && s.IsConnected);
            }


            //
            // epObservable
            //

            //connect (multiple connects)
            //connect (multiple connects, handler has fix blocking time) 
            TestUtils.AssertTask(barrier.WaitAsync(), 10000);


            //connect (handler throws exception)
            TestUtils.AssertTask(tcs.Task);
            Task.Delay(10).Wait();            
            Assert.IsFalse(exceptionEndpoint.Socket.IsConnected);


            //  disconnect client (5)
            epObservable.EndpointDisconnectedHandler = (ep, remote) =>
            {
                return Task.CompletedTask;
            };

            //  disconnect (4)

            var ep = endpoints.First();
            TestUtils.AssertTask(epControl.DisconnectEndpointAsync(ep));
            Assert.IsFalse(ep.Socket.IsConnected);

            //disconnect all
            TestUtils.AssertTask(epControl.DisconnectAllAsync());
            foreach(var endpoint in endpoints)
            {
                Assert.IsFalse(endpoint.Socket.IsConnected);
            }




        }

    }
}
