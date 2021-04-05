using MessageUtilities;
using MessageUtility.MessageDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyModule.GameManagementModule.Messages
{
    public class GameMessageFactory
    {
        private static GameMessageFactory _instance;
        private static readonly MessageDictionary Messages = MessageDictionary.GetInstance();

        public static GameMessageFactory GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GameMessageFactory();
            }
            return _instance;
        }

        private GameMessageFactory()
        {
            Messages.AddOutgoingMessage("Game", "Initialize");
            Messages.AddOutgoingMessage("Game", "Start");
            Messages.AddOutgoingMessage("Game", "GameOver");
        }

        internal Message Initialize()
        {
            return Messages.CreateMessage("Initialize");
        }

        internal Message Start()
        {
            return Messages.CreateMessage("Start");
        }

        internal Message GameOver()
        {
            return Messages.CreateMessage("GameOver");
        }
    }
}
