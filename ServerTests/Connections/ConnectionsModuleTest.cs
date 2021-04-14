using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtils.Endpoint;
using NetworkUtils.Socket;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using SererTests.TestUtilities;
using ServerKernel.Connections;
using ServerKernel.Connections.Manager;
using ServerKernel.Data_Objects;
using ServerTests.Mocks;
using ServerUtils;
using SyncUtils;
using SyncUtils.Barrier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServerTests.Connections
{
    [TestClass]
    public class ConnectionsModuleTest
    {
        class MockClientProcessorModule : ModuleControl, IConnectionProcessor
        {
            public Func<Connection, Task> DisconnectHandler { get; set; }
            public Func<Connection, Task> SynchronizeHandler { get; set; }

            public MockClientProcessorModule() : base("processor", PatternUtils.Version.Create(1))
            {
            }

            public Task Disconnect(Connection connection)
            {
                return DisconnectHandler?.Invoke(connection);
            }

            public Task Synchronize(Connection connection)
            {
                return SynchronizeHandler?.Invoke(connection);
            }

            protected override void DefineModule(ModuleBuilder builder)
            {
                builder.SetManagedInterfaces(new ManagedInterfaceWrapper<IConnectionProcessor>(this, PatternUtils.Version.Create(1)));
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

        [TestMethod]
        public void TestConnectionProcessor()
        {
            const int COUNT = 10;

            SemaphoreSlim sem = new(1, 1);
            ModuleManager manager = new();

            MockBaseModule baseModule = new();
            ConnectionModule connectionModule = new();
            MockClientProcessorModule processorModuleA = new();
            MockClientProcessorModule processorModuleB = new();

            TestUtils.AssertTask(manager.RegisterModuleAsync(baseModule.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(connectionModule.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(processorModuleA.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(processorModuleB.Header));

            AsyncBarrier barrier = new(2*COUNT, false);

            //connect multiple endpoints with 2 managed interfaces

            processorModuleA.DisconnectHandler = c =>
            {
                Assert.IsTrue(false);
                return Task.CompletedTask;
            };
            processorModuleA.SynchronizeHandler = async c =>
            {
                await barrier.SignalAsync();
            };

            processorModuleB.DisconnectHandler = c =>
            {
                Assert.IsTrue(false);
                return Task.CompletedTask;
            };
            processorModuleB.SynchronizeHandler = async c =>
            {
                await barrier.SignalAsync();
            };

            List<MockEndpoint> endpoints = new();
            TestUtils.AssertTask(AsyncUtilities.RepeatAsync(async () =>
            {
                var ep = new MockEndpoint();

                await sem.WaitAsync();
                endpoints.Add(ep);
                sem.Release();

                baseModule.ConnectEndpoint(ep);
            }, COUNT), 1000);

            TestUtils.AssertTask(barrier.WaitAsync(), 1000);

            //disconnect one endpoint with 2 managed interfaces

            barrier = new(2, false);

            processorModuleA.DisconnectHandler = async c =>
            {
                await barrier.SignalAsync();
            };
            processorModuleA.SynchronizeHandler = c =>
            {
                Assert.IsTrue(false);
                return Task.CompletedTask;
            };

            processorModuleB.DisconnectHandler = async c =>
            {
                await barrier.SignalAsync();
            };
            processorModuleB.SynchronizeHandler = c =>
            {
                Assert.IsTrue(false);
                return Task.CompletedTask;
            };

            endpoints.First().Disconnect(true);
            TestUtils.AssertTask(barrier.WaitAsync());
            endpoints.Remove(endpoints.First());

            //disconnect all remaining endpoints after unregistering one of the managed interfaces
            TestUtils.AssertTask(manager.UnregisterModuleAsync(processorModuleB.Header));

            processorModuleB.DisconnectHandler = c =>
            {
                Assert.IsTrue(false);
                return Task.CompletedTask;
            };
            barrier = new(COUNT-1, false);
            TestUtils.AssertTask(AsyncUtilities.ForEachAsync( (ep) =>
            {
                ep.Disconnect(true);
                return Task.CompletedTask;
            }, endpoints), 1000);

            TestUtils.AssertTask(barrier.WaitAsync());

        }


        [TestMethod]
        public void TestIncomingConnections()
        {
            const int COUNT = 10;
            ModuleManager manager = new();

            MockBaseModule baseModule = new();
            ConnectionModule connectionModule = new();


            //register modules and get interface
            TestUtils.AssertTask(manager.RegisterModuleAsync(baseModule.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(connectionModule.Header));

            var connectionControl = TestUtils.AssertTask(manager.GetInterfaceAsync<IConnectionControl>(null));

            //connect endpoints
            AsyncBarrier barrier = new(COUNT);

            baseModule.MockProtocol.NewConnection = (connection) =>
            {
                Assert.AreEqual(ConnectionState.Connecting, connection.State);
                return Task.CompletedTask;
            };
            baseModule.MockProtocol.SynchronizingConnection = (connection) =>
            {
                Assert.AreEqual(ConnectionState.Synchronizing, connection.State);
                return Task.CompletedTask;
            };
            baseModule.MockProtocol.EstablishedConnection = async (connection) =>
            {
                Assert.AreEqual(ConnectionState.Established, connection.State);
                await barrier.SignalAsync();
            };

            List<Task> tasks = new();
            List<MockEndpoint> endpoints = new();
            for(int i = 0; i < COUNT; i++)
            {
                var endpoint = new MockEndpoint();
                endpoints.Add(endpoint);
                tasks.Add(Task.Run(() => baseModule.ConnectEndpoint(endpoint)));
            }
            Task.WhenAll(tasks).Wait();

            TestUtils.AssertTask(barrier.WaitAsync(), 500);

            Assert.AreEqual(endpoints.Count, baseModule.ConnectedEndpoints.Count);
            Assert.AreEqual(endpoints.Count, connectionControl.Connections.Count);
            for (int i = 0; i < COUNT; i++)
            {
                var connection = (Connection)endpoints[i].ConnectionData;

                Assert.IsTrue(connectionControl.Connections.Contains(connection));
                Assert.IsNotNull(connection);
                Assert.AreEqual(endpoints[i], connection.Endpoint);
            }

            //disconnect local
            barrier = new AsyncBarrier(2, false);
            baseModule.MockProtocol.ClosingConnection = async (connection, reason, msg) =>
            {
                //only called for local disconnect
                Assert.AreEqual(ConnectionState.Established, connection.State);
                await barrier.SignalAsync();
            };
            baseModule.MockProtocol.DisconnectedConnection = async (connection) =>
            {
                Assert.AreEqual(ConnectionState.Disconnected, connection.State);
                await barrier.SignalAsync();
            };
            connectionControl.CloseConnectionAsync((Connection)endpoints[0].ConnectionData, DisconnectReason.Kick, "test");

            TestUtils.AssertTask(barrier.WaitAsync(), 500);

            Assert.IsFalse(endpoints[0].Socket.IsConnected);

            endpoints.RemoveAt(0);

            foreach (var ep in endpoints)
            {
                Assert.IsTrue(ep.Socket.IsConnected);
            }

            Assert.AreEqual(endpoints.Count, baseModule.ConnectedEndpoints.Count);
            Assert.AreEqual(endpoints.Count, connectionControl.Connections.Count);

            //disconnect remote
            barrier = new AsyncBarrier(1, false);
            endpoints[0].Disconnect(true);

            TestUtils.AssertTask(barrier.WaitAsync(), 500);

            Assert.IsFalse(endpoints[0].Socket.IsConnected);

            endpoints.RemoveAt(0);

            foreach (var ep in endpoints)
            {
                Assert.IsTrue(ep.Socket.IsConnected);
            }

            Task.Delay(1000);

            barrier = new AsyncBarrier(2*endpoints.Count, false);

            Assert.AreEqual(endpoints.Count, baseModule.ConnectedEndpoints.Count);
            Assert.AreEqual(endpoints.Count, connectionControl.Connections.Count);

            //disconnect all
            TestUtils.AssertTask(connectionControl.CloseAllConnectionsAsync());
            
            foreach (var ep in endpoints)
            {
                Assert.IsFalse(ep.Socket.IsConnected);
            }


        }

    }
}
