using DiagnosticsModule;
using DiagnosticsModule.Interface;
using System;
using System.Collections.Generic;

namespace PlayerModule.Data
{
    public class ConnectionPlayerInfo : IDiagnosticDataObject
    {
        public Player MainPlayer { get; }
        public IReadOnlyCollection<Player> Players => PlayerDic.Values;
        public Dictionary<int, Player> PlayerDic { get; } = new Dictionary<int, Player>();

        public ConnectionPlayerInfo(Player mainPlayer)
        {
            MainPlayer = mainPlayer;
            PlayerDic.Add(mainPlayer.Id, mainPlayer);
        }

        public string ToDiagnosticLongString()
        {
            string output = "main: " + MainPlayer.Id + ", ";
            output += DiagnosticsUtils.FlatString("player", Players);
            return output;
        }

        public string ToDiagnosticShortString()
        {
            throw new NotImplementedException();
        }
    }
}
