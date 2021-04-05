using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiagnosticsModule.Interface;
using GameManagementModule;
using ServerImplementation.Modules;

namespace GameManagement.Module
{
    public class GameManagementModule : Module<GameManagementLogic, GameManagementData, GameManagementMessageHandler>, IDiagnosticsControl
    {
        public GameManagementModule() : base("Game", "Game Management")
        {
        }

        public IDiagnosticsDataInterface Diagnostics => Data;
    }
}
