using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiagnosticsModule.Interface;
using ServerImplementation.Modules;

namespace GameManagement.GameStorageModule
{
    public class GameStorageModule : Module<GameStorageLogic, GameStorageData, GameStorageMessageHandler>, IDiagnosticsControl
    {
        public GameStorageModule() : base("Game Storage", "Game Storage")
        {
        }

        public IDiagnosticsDataInterface Diagnostics => Data;
    }
}
