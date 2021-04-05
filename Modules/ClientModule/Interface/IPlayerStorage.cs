using PlayerModule.Data;
using GameServer.Data_Objects;
using System.Collections.Generic;
using System;

namespace PlayerModule.Interface
{
    /**
     * After synchronization, each connection is linked to a ConnectionPlayerInfo object.
     * Each Connection has at least one corresponding Player object (initially called "Player {Id}")
     * Any number of Player's can be added to a connection. 
     * 
     */

    public interface IPlayerStorage
    {
        event Action<Player> PlayerRemoved;
        IReadOnlyCollection<Player> Players { get; }
        bool IsSynchronized(IConnection connection);
        ConnectionPlayerInfo GetPlayerInfo(IConnection connection);
        Player GetPlayerById(int id);
        IReadOnlyCollection<Player> GetPlayerOfConnection(IConnection c);
        bool PlayerExists(int id);
    }
}
