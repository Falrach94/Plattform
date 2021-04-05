using ControlModule;
using GameServer;
using GameServer.Data_Objects;
using LogUtils;
using System;
using System.Collections.Generic;

namespace ServerImplementation.ControlModule
{
    public class ControlData : IModuleData
    {
        public IServer Server { get; set; }
        public IDataGuard DataGuard { get; set; }

        private readonly IDictionary<int, ServerRightsLevel> _rightsLevelDic = new Dictionary<int, ServerRightsLevel>();
        private int _superUserId = -1;

     //   private static readonly Logger log = Logging.LogManager.GetCurrentClassLogger();
        public void RemoveAccessEntry(Access access, int id)
        {
            if (!access.HasWriteAccess)
            {
                throw new Exception("Unsufficient access rights!");
            }
            _rightsLevelDic.Remove(id);
        }

        public void SetAccessLevel(Access access, int id, ServerRightsLevel level)
        {
            if (!access.HasWriteAccess)
            {
                throw new Exception("Unsufficient access rights!");
            }
            _rightsLevelDic[id] = level;
        }

        public ServerRightsLevel GetAccessLevel(Access access, int id)
        {
            if (!access.HasReadAccess)
            {
                throw new Exception("Unsufficient access rights!");
            }
            return _rightsLevelDic[id];
        }

        public bool IsSuperUserSet(Access access)
        {
            if (!access.HasReadAccess)
            {
                throw new Exception("Unsufficient access rights!");
            }
            return _superUserId != -1;
        }
        public int GetSuperUser(Access access)
        {
            if (!access.HasReadAccess)
            {
                throw new Exception("Unsufficient access rights!");
            }
            return _superUserId;
        }

        public void SetSuperUser(Access access, int id)
        {
            if (!access.HasWriteAccess)
            {
                throw new Exception("Unsufficient access rights!");
            }

            _superUserId = id;

          //  log.Info("Gave super user priviliges to connection " + id);

        }


        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized(IConnection con)
        {
            throw new NotImplementedException();
        }
    }
}
