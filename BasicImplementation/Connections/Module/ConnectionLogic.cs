using GameServer;
using GameServer.Data_Objects;
using GameServer.Network;
using NetworkUtils.Socket;
using ServerImplementation.Client;
using ServerImplementation.ConnectionHandler;
using ServerImplementation.Exceptions;
using ServerImplementation.Modules;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServerImplementation.ConnectionModule
{
    public class ConnectionLogic : ModuleLogic<ConnectionData>, IConnectionManager
    {

        private static readonly GeneralMessageFactory Messages = GeneralMessageFactory.GetInstance();

        #region properties
        IConnectionStorage IConnectionManager.Data => Data;
        public IModuleControl Modules { get; set; }
        #endregion

        public override async Task Reset()
        {
            var connections = Data.Connections.ToArray();
            foreach (var c in connections)
            {
                await CloseConnection(c, DisconnectReason.ServerClosed, "Server closed!");
            }
        }

        #region endpoint proccessing


        #endregion

    }
}