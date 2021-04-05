using GameManagement;
using GameManagement.GameStorageModule.Data;
using GameManagement.LobbyM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManagement
{
    public interface IGameManagement    
    {
        Task StartGame(Lobby lobby);
        Task AbortGame(GameInstance game);
    }
}