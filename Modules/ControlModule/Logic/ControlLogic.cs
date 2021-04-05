using ControlModule;
using ControlModule.Logic;
using GameServer.Data_Objects;
using GameServer.Network;
using GameServerData;
using MessageUtilities;
using ServerImplementation.ControlModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameServer
{
    public class ControlLogic : IModuleLogic, IServerControl, IServerInfo
    {
        #region properties
        public ControlData Data { get; }
        public IMessenger Messenger { get; set; }
        public IServer Server { get; set; }

        IModuleData IModuleLogic.Data => Data;

        public IReadOnlyCollection<IModule> Modules => Server.Framework.Modules;

        public IReadOnlyCollection<IConnection> Connections => Server.ConnectionStorage.Connections;

        #endregion

        public ControlLogic(ControlData data)
        {
            Data = data;
        }

        public void Initialize()
        {
        }

        public Task ShutDown()
        {
            return Task.CompletedTask;
        }


        public void CloseServer()
        {
            Data.Server.ShutDown();
        }

        public void Kick(int id, string msg)
        {
            var connection = Server.ConnectionStorage.GetConnectionById(id);
            Server.ConnectionHandler.CloseConnection(connection, DisconnectReason.Kick, msg);
        }

        public void SetAccessLevel(int connectionId, ServerRightsLevel level)
        {
            throw new NotImplementedException();
        }


        Task IModuleLogic.Broadcast(Message msg)
        {
            throw new NotImplementedException();
        }

        Task IModuleLogic.ShutDown()
        {
            throw new NotImplementedException();
        }

        Task IModuleLogic.HandleDisconnect(IConnection connection)
        {
            throw new NotImplementedException();
        }

        Task IModuleLogic.SynchronizeConnection(IConnection connection)
        {
            throw new NotImplementedException();
        }

        public Task Reset()
        {
            return Task.CompletedTask;
        }
    }
}