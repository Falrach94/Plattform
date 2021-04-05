using GameServer;
using GameServer.Data_Objects;
using GameServer.Network;
using GameServerData;
using ServerImplementation.Modules;
using System;
using System.Linq;
using System.Threading.Tasks;
using TextInterfaceModule.Interface;
using TextInterfaceModule.Logic.Parser;
using TextParser.Parser;
using TextParserUtils.Parser;

namespace TextInterfaceModule
{
    /**
     * 
     */

    public class TextInterfaceLogic : ModuleLogic<TextInterfaceData>, ICommandInterface, ITextInterfaceControl
    {

        private readonly TextCommandParser _parser = new TextCommandParser();

        private string Kick(int id, string msg)
        {
            try
            {
                var connection = Server.ConnectionStorage.GetConnectionById(id);
                Server.ConnectionHandler.CloseConnection(connection, DisconnectReason.Kick, msg);
                return "Client with id " + id + " was kicked from server!";
            }
            catch
            {
                return "No client with id " + id;
            }
        }

        public TextInterfaceLogic()
        {
            AddToken("Start", "^start");
            AddToken("Close", "^close");
            AddToken("Server", "^server");
            AddToken("Modules", "^modules");
            AddToken("Connections", "^connections");
            AddToken("Kick", "^kick");
            AddToken("Help", "^help");
            AddToken("Reset", "^reset");

            /*
                AddRule(new CommandParserRule(
                    ParserSequence.Create(),
                    (context) =>
                    {
                    }));
            */

            AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Reset").End(),
                (c) => ResetCommand(Server.Port)));
            AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Reset").Number("Port").End(),
                (c) => ResetCommand(c.Get<int>("Port"))));
            AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Start").Token("Server").End(),
                (c) => StartServer(100)));
            AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Start").Token("Server").Number("Port").End(),
                (c) => StartServer(c.Get<int>("Port"))));
            AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Close").Token("Server").End(),
                (context) =>
                {
                    Server.ShutDown();

                    return "Server closed!";
                }));

            AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Connections").End(),
                (context) =>
                {
                    string result = "Connections (" + Server.ConnectionStorage.Connections.Count + ")";
                    foreach (var c in Server.ConnectionStorage.Connections)
                    {
                        result += "\n   " + c.ToString();
                    }
                    return result;
                }));

            AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Kick").Number("Id").End(),
                (context) =>
                {
                    return Kick(context.Get<int>("Id"), null);
                }));
            AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Kick").Number("Id").String("Msg").End(),
                (context) =>
                {
                    return Kick(context.Get<int>("Id"), context.Get<string>("Msg"));
                }));

            AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Help").End(),
                (context) =>
                {
                    return "valid commands:\n   " + String.Join("\n   ", _parser.Examples);
                }));

            AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Modules").Number("Id").End(),
                (context) =>
                {
                    return ConnectionModules(context.Get<int>("Id"));
                }));

            AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Modules").End(),
                (context) =>
                {
                    return "loaded modules:\n   " + String.Join("\n   ", Server.Framework.Modules.Select(m => m.Name));
                }));
        }

        private string ResetCommand(int port)
        {
            Server.Reset(port).Wait();
            return "Reset server!";
        }

        private string StartServer(int port)
        {
            Server.Start(port).Wait();
            return "Server started on port " + port + "!";
        }

        private string ConnectionModules(int connectionId)
        {
            if (!Server.ConnectionStorage.ConnectionExists(connectionId))
            {
                return "No connection with id " + connectionId + " found!";
            }
            var connection = Server.ConnectionStorage.GetConnectionById(connectionId);

            return "module support of connection " + connectionId + ":\n   " + String.Join("\n   ", connection.ModuleDic.Select(p => p.Key + ": " + (p.Value ? "supported" : "not supported")));

        }

        public string Command(string command)
        {
            var result = _parser.ParseCommand(command);

            if (result.Success)
            {
                try
                {
                    return result.Value(result.Context);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
            {
                return "Command '" + command + "' not recognized!";
            }
        }


        public void AddToken(string token, string regex)
        {
            _parser.Tokenizer.AddToken(token, regex);
        }

        public void AddRule(CommandParserRule rule)
        {
            _parser.AddRule(rule);
        }

        public void AddRule(ParserSequence sequence, CommandDelegate command, string example)
        {
            AddRule(new CommandParserRule(sequence, command, example));
        }
        public void AddRule(ParserSequence sequence, CommandDelegate command)
        {
            AddRule(new CommandParserRule(sequence, command, null));
        }

        public override Task Reset()
        {
            return Task.CompletedTask;
        }
    }
}