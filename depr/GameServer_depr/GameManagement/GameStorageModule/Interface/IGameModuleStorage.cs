using GameManagement.GameStorageModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManagement.Interface
{
    public interface IGameModuleStorage
    {
        IReadOnlyDictionary<int, GameWrapper> Games { get;}

        void AddGame(IServerGameFactory game);

        void LoadDll(string path);
    }
}
