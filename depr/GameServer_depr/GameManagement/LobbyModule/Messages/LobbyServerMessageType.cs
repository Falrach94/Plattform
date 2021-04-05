using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameManagement
{
    public enum LobbyServerMessageType
    {
        Lobbies,
        LobbyOptionSet,
        PlayerJoinedLobby,
        PlayerLeftLobby,
        LobbyClosed
    }
}