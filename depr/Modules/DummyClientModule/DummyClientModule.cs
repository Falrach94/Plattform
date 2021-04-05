using DiagnosticsModule.Interface;
using DummyClientModule.Client;
using GameServer;
using LogModules.Interface;
using ServerImplementation.Modules;
using System;
using TextInterfaceModule.Interface;
using TextParser.Parser;

namespace DummyClientModule
{
    public class DummyClientModule : Module<DummyClientLogic, DummyClientData, DummyClientMessageHandler>, IRegisterTextInterface, IDiagnosticsControl
    {
        public IDiagnosticsDataInterface Diagnostics => Data;

        public DummyClientModule() : base("Dummy Module", "Dummy Module")
        {
        }

        public override void GreetModule(IModule module)
        {
            if(module is ILogModule logModule)
            {
                Logic.LogStream = logModule.LogManager.LogStream;
            }
        }

        public void RegisterTextInterface(ITextInterfaceControl textInterface)
        {
            textInterface.AddToken("Dummies", "^dummies");
            textInterface.AddToken("Dummy", "^dummy");
            textInterface.AddToken("Create", "^create");
            textInterface.AddToken("Remove", "^remove");
            textInterface.AddToken("Connect", "^connect");
            textInterface.AddToken("Disconnect", "^disconnect");

            textInterface.AddRule(ParserSequence.Start().Token("Dummies").End(),
                (c) =>
                {
                    var msg = "Dummies (" + Data.ClientDic.Count + ")";
                    if (Data.ClientDic.Count != 0)
                    {
                        msg += "\n   ";
                    }
                    msg += String.Join("\n   ", Data.ClientDic.Values);
                    return msg;
                });
            textInterface.AddRule(ParserSequence.Start().Token("Create").Token("Dummy").End(),
                (c) =>
                {
                    return CreateDummy(null, true);
                });
            textInterface.AddRule(ParserSequence.Start().Token("Create").Token("Dummies").Number("Count").End(),
                (c) =>
                {
                    for (int i = 0; i < c.Get<int>("Count"); i++)
                    {
                        CreateDummy(null, true);
                    }
                    return "Created and connected dummies!";
                });
            textInterface.AddRule(ParserSequence.Start().Token("Create").Token("Dummy").Boolean("Connect").End(),
                (c) =>
                {
                    return CreateDummy(null, c.Get<bool>("Connect"));
                });
            textInterface.AddRule(ParserSequence.Start().Token("Create").Token("Dummy").String("Name").End(),
                (c) =>
                {
                    return CreateDummy(c.Get<string>("Name"), true);
                });
            textInterface.AddRule(ParserSequence.Start().Token("Create").Token("Dummy").String("Name").Boolean("Connect").End(),
                (c) =>
                {
                    return CreateDummy(c.Get<string>("Name"), c.Get<bool>("Connect"));
                });
            textInterface.AddRule(ParserSequence.Start().Token("Remove").Token("Dummy").String("Name").End(),
                (c) =>
                {
                    return RemoveDummy(c.Get<string>("Name"));
                });
            textInterface.AddRule(ParserSequence.Start().Token("Remove").Token("Dummy").Number("Id").End(),
                (c) =>
                {
                    return RemoveDummy(c.Get<int>("Id"));
                });
            textInterface.AddRule(ParserSequence.Start().Token("Connect").Token("Dummy").Number("Id").End(),
                (c) =>
                {
                    return ConnectDummy(c.Get<int>("Id"));
                });
            textInterface.AddRule(ParserSequence.Start().Token("Connect").Token("Dummy").String("Name").End(),
                (c) =>
                {
                    return ConnectDummy(c.Get<string>("Name"));
                });
            textInterface.AddRule(ParserSequence.Start().Token("Disconnect").Token("Dummy").Number("Id").End(),
                (c) =>
                {
                    return DisconnectDummy(c.Get<int>("Id"));
                });
            textInterface.AddRule(ParserSequence.Start().Token("Disconnect").Token("Dummy").String("Name").End(),
                (c) =>
                {
                    return DisconnectDummy(c.Get<string>("Name"));
                });
        }

        private string DisconnectDummy(string name)
        {
            var dummy = Data.GetClient(name);

            if (dummy == null)
            {
                return "No dummy with this name found!";
            }

            Logic.DisconnectClient(dummy);
            return "Successfully disconnected dummy " + dummy + "!";
        }

        private string DisconnectDummy(int id)
        {

            if (!Data.ClientDic.TryGetValue(id, out DummyClient dummy))
            {
                return "No dummy with this id found!";
            }

            Logic.DisconnectClient(dummy);
            return "Successfully disconnected dummy " + dummy + "!";
        }

        private string ConnectDummy(string name)
        {
            var dummy = Data.GetClient(name);

            if (dummy == null)
            {
                return "No dummy with this name found!";
            }

            Logic.ConnectClient(dummy);
            return dummy + " started connect!";
        }

        private string ConnectDummy(int id)
        {

            if (!Data.ClientDic.TryGetValue(id, out DummyClient dummy))
            {
                return "No dummy with this id found!";
            }

            Logic.ConnectClient(dummy);
            return dummy + " started connect!";
        }

        private string RemoveDummy(string name)
        {
            var dummy = Data.GetClient(name);

            if (dummy == null)
            {
                return "No dummy with this name found!";
            }

            Logic.RemoveClient(dummy);
            return "Successfully removed dummy " + dummy + "!";
        }

        private string RemoveDummy(int id)
        {

            if (!Data.ClientDic.TryGetValue(id, out DummyClient dummy))
            {
                return "No dummy with this id found!";
            }

            Logic.RemoveClient(dummy);
            return "Successfully removed dummy " + dummy + "!";
        }

        private string CreateDummy(string name, bool connect)
        {
            var dummy = Logic.CreateClient(name);

            if (connect)
            {
                Logic.ConnectClient(dummy);
            }

            return "Created dummy " + dummy + "!";
        }
    }
}
