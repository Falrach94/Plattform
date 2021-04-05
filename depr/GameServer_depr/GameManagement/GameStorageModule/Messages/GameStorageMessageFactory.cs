using GameManagement.GameStorageModule.Data;
using MessageUtility.MessageDictionary;
using System;

namespace GameManagement.GameStorageModule
{
    internal class GameStorageMessageFactory
    {
        private static GameStorageMessageFactory _instance;

        internal static GameStorageMessageFactory GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GameStorageMessageFactory();
            }
            return _instance;
        }

        private GameStorageMessageFactory()
        {
        }


    }
}