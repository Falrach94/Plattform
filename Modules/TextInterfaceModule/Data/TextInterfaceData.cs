using ControlModule;
using ControlModule.Logic;
using GameServer;
using GameServer.Data_Objects;
using ServerImplementation.Modules;
using System;

namespace TextInterfaceModule
{
    public class TextInterfaceData : ModuleData
    {

        public IServerControl Control { get; internal set; }
        public IServerInfo Info { get; internal set; }

    }
}
