using LobbyModule.Game_Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.GameSettings;

namespace DummyGame
{
    public class DefaultModeInfo : GameModeInfo<DefaultMode>
    {
        public DefaultModeInfo() 
            : base("default", 2, 2)
        {
        }

        protected override void RegisterSettings(ISettingsBuilder builder)
        {
            builder.AddIntegerSetting("size", 3, 5);
        }
    }
}
