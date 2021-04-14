using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Extensions;
using SererTests.TestUtilities;
using ServerKernel.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTests.Server
{
    [TestClass]
    public class ServerTest
    {
        [TestMethod]
        public void TestServerBasics()
        {
            const int PORT_INIT = 4234;
            const int PORT = 1234;

            ServerKernel.Server server = new();

            //
            // assert modules network, connections, control, messaging are loaded
            // assert modules can be started
            // assert correct port is used
            // assert modules can be stopped
            // assert port can be changed
            //


            // assert modules network, connections, control, messaging are loaded
            CollectionAssert.AreEquivalent(new[] { "Connections", "Messaging", "Control", "Network"}, 
                                            server.ModuleManager.Modules.Select(m => m.Info.Name).ToArray());

            var networkModule = server.ModuleManager.GetModuleByName("Network");
            var controlModule = server.ModuleManager.GetModuleByName("Control");
            var messagingModule = server.ModuleManager.GetModuleByName("Messaging");
            var connectionsModule = server.ModuleManager.GetModuleByName("Connections");

            var network = TestUtils.AssertTask(server.ModuleManager.GetInterfaceAsync<INetworkControl>(null));

            Assert.AreEqual(ModuleState.Stopped, networkModule.State);
            Assert.AreEqual(ModuleState.Stopped, controlModule.State);
            Assert.AreEqual(ModuleState.Stopped, messagingModule.State);
            Assert.AreEqual(ModuleState.Stopped, connectionsModule.State);

            // assert modules can be started
            TestUtils.AssertTask(server.StartAsync(PORT_INIT));

            Assert.AreEqual(ModuleState.Running, networkModule.State);
            Assert.AreEqual(ModuleState.Running, controlModule.State);
            Assert.AreEqual(ModuleState.Running, messagingModule.State);
            Assert.AreEqual(ModuleState.Running, connectionsModule.State);

            // assert correct port is used
            Assert.AreEqual(PORT_INIT, network.Port);

            // assert modules can be stopped
            TestUtils.AssertTask(server.StopAsync());

            Assert.AreEqual(ModuleState.Stopped, networkModule.State);
            Assert.AreEqual(ModuleState.Stopped, controlModule.State);
            Assert.AreEqual(ModuleState.Stopped, messagingModule.State);
            Assert.AreEqual(ModuleState.Stopped, connectionsModule.State);

            // assert port can be changed
            TestUtils.AssertTask(server.StartAsync(PORT));

            Assert.AreEqual(ModuleState.Running, networkModule.State);
            Assert.AreEqual(ModuleState.Running, controlModule.State);
            Assert.AreEqual(ModuleState.Running, messagingModule.State);
            Assert.AreEqual(ModuleState.Running, connectionsModule.State);

            Assert.AreEqual(PORT, network.Port);



            // server.ModuleManager.assert

        }
    }
}
