using Microsoft.VisualStudio.TestTools.UnitTesting;
using SererTests.TestUtilities;
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

           // TestUtils.AssertTask(server.StartAsync(PORT));


           // server.ModuleManager.assert

        }
    }
}
