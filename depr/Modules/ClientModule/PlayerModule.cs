using PlayerModule.Interface;
using DiagnosticsModule.Interface;
using ServerImplementation.Modules;
using System;
using System.Linq;
using System.Threading.Tasks;
using TextInterfaceModule.Interface;
using TextInterfaceModule.Logic.Parser;
using TextParser.Parser;

namespace GameServer
{
    public class PlayerModule : Module<PlayerLogic, PlayerModuleData, PlayerMessageHandler>, IPlayerModule, IRegisterTextInterface, IDiagnosticsControl
    {
        public IPlayerStorage PlayerStorage => Data;

        public IDiagnosticsDataInterface Diagnostics => Data;

        private static PlayerModule _instance;

        private PlayerModule()
            : base("Player", "Player-Module")
        {
        }

        public static PlayerModule GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PlayerModule();
            }
            return _instance;
        }

        public void RegisterTextInterface(ITextInterfaceControl textInterface)
        {
            textInterface.AddToken("Players", "^players");
            textInterface.AddToken("Add", "^add");
            textInterface.AddToken("Remove", "^remove");
            textInterface.AddToken("Rename", "^rename");
            textInterface.AddToken("Player", "^player");

            /*
            textInterface.AddRule(new CommandParserRule(
                ParserSequence.Create(),
                (c) =>
                {
                },
                "example"));
            */

            textInterface.AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Player").End(),
                (c) =>
                {
                    return PlayersCommand();
                }));
            textInterface.AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Player").Number("Id").End(),
                (c) =>
                {
                    return ConnectionPlayersCommand(c.Get<int>("Id"));
                }));

            textInterface.AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Player").Token("Add").String("Name").Number("Id").End(),
                (c) =>
                {
                    return AddPlayerCommand(c.Get<string>("Name"), c.Get<int>("Id"));
                }));

            textInterface.AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Player").Token("Remove").Number("Id").End(),
                (c) =>
                {
                    return RemovePlayerCommand(c.Get<int>("Id"));
                }));

            textInterface.AddRule(new CommandParserRule(
                ParserSequence.Start().Token("Player").Token("Rename").Number("Id").String("Name").End(),
                (c) =>
                {
                    return RenamePlayerCommand(c.Get<int>("Id"), c.Get<string>("Name"));
                }));

        }

        private string RenamePlayerCommand(int id, string name)
        {
            if (!PlayerStorage.PlayerExists(id))
            {
                return "No Player with id " + id + " found!";
            }

            var player = PlayerStorage.GetPlayerById(id);

            string oldName = player.Name;

            Logic.RenamePlayer(player, name).Wait();

            return "Player '" + oldName + "' was renamed to '" + name + "'!";

        }

        private string RemovePlayerCommand(int id)
        {
            if (!PlayerStorage.PlayerExists(id))
            {
                return "No Player with id " + id + " found!";
            }

            var player = PlayerStorage.GetPlayerById(id);
            var connection = player.Connection;

            try
            {
                Logic.RemovePlayer(connection, player).Wait();
            }
            catch (PlayerRemovalException ex)
            {
                return ex.Message;
            }
            return "Player '" + player.Name + "' was removed!";
        }

        private string AddPlayerCommand(string name, int id)
        {
            if (!Server.ConnectionStorage.ConnectionExists(id))
            {
                return "No connection with id " + id + " found!";
            }

            var connection = Server.ConnectionStorage.GetConnectionById(id);

            if (!Data.IsSynchronized(connection))
            {
                return "Connection is not synchronized with player module!";
            }

            Logic.AddPlayer(connection, name).Wait();

            return "Player '" + name + "' added to connection with id " + id + "!";

        }

        private string ConnectionPlayersCommand(int id)
        {
            if (!Server.ConnectionStorage.ConnectionExists(id))
            {
                return "No connection with id " + id + " found!";
            }

            var connection = Server.ConnectionStorage.GetConnectionById(id);

            if (Data.IsSynchronized(connection))
            {
                return "Connection is not synchronized with player module!";
            }

            var playerInfo = Data.GetPlayerInfo(connection);


            string output = "Players on connection " + id + " (" + playerInfo.Players.Count + ")";

            if (playerInfo.Players.Count != 0)
            {
                output += "\n   ";
            }

            output += String.Join("\n   ", playerInfo.Players.Select(p => p.ToLongString()));

            return output;
        }

        private string PlayersCommand()
        {
            string output = "Players (" + Data.Players.Count + ")";

            if (Data.Players.Count != 0)
            {
                output += "\n   ";
            }

            output += String.Join("\n   ", Data.Players.Select(p => p.ToLongString()));

            return output;
        }

    }
}