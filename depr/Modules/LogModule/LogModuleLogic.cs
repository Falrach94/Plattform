using GameServer;
using LogModules.Interface;
using LogUtils;
using ServerImplementation.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace LogModule
{
    public class LogModuleLogic : ModuleLogic<LogModuleData>, ILogManager
    {
        #region fields
        private readonly CancellationTokenSource _cancelSource = new CancellationTokenSource();

        private Thread _logThread;

        #endregion

        public Stream LogStream { get; } = new MemoryStream();

        public event EventHandler<LogMessage> NewMessage;

        public List<LogMessage> GetLog(string instance, LogCategory[] categories)
        {
            Dictionary<string, LogLevel> levelDic = new Dictionary<string, LogLevel>();
            foreach (var c in categories)
            {
                levelDic.Add(c.Name, c.MinLevel);
            }

            return Data.Logs[instance].Where(m => levelDic.ContainsKey(m.LogModule) && m.Level >= levelDic[m.LogModule]).ToList();
        }



        public override void Initialize()
        {
            Server.LogManager.Stream = LogStream;

            base.Initialize();
        }

        public void StartLogThread()
        {
            if(_logThread != null)
            {
                throw new Exception("Log thread may only be started once!");
            }

            _logThread = new Thread(LogHandler);
            _logThread.Start();
        }

        public void StopLogThread()
        {
            _cancelSource.Cancel();
            _logThread = null;
        }

        private void LogHandler()
        {
            BinaryFormatter bf = new BinaryFormatter();

            while (!_cancelSource.Token.IsCancellationRequested || LogStream.Length != 0)
            {
                List<LogMessage> logMessages = new List<LogMessage>();

                lock (LogStream)
                {
                    while (LogStream.Length == 0)
                    {
                        Monitor.Wait(LogStream);
                    }

                    LogStream.Position = 0;


                    while (LogStream.Length != LogStream.Position)
                    {
                        LogMessage msg;
                        msg = (LogMessage)bf.Deserialize(LogStream);

                        logMessages.Add(msg);

                    }

                    LogStream.SetLength(0);
                }

                foreach (var m in logMessages)
                {
                    Data.AddLogMessage(m);
                    NewMessage?.Invoke(this, m);
                }
            }
        }
        public override Task Reset()
        {
            return Task.CompletedTask;
        }
    }
}