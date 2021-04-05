using LogUtils;
using System;
using System.Collections.Generic;
using System.IO;

namespace LogModules.Interface
{
    public interface ILogManager
    {
        Stream LogStream { get; }

        event EventHandler<LogMessage> NewMessage;

        List<LogMessage> GetLog(string instance, LogCategory[] categories);

        void StartLogThread();
        void StopLogThread();
    }
}
