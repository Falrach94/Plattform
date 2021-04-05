using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyModule.LobbyModule.Data
{
    public interface ISetting
    {
        string Name { get; }

        bool ValidateValue(object value);
    }
}
