using LogUtils;
using ServerImplementation.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LogModule
{
    public class LogModuleData : ModuleData
    {
        private readonly Dictionary<string, List<LogMessage>> _logs = new Dictionary<string, List<LogMessage>>();

        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public IReadOnlyDictionary<string, List<LogMessage>> Logs => _logs;

        public async void AddLogMessage(LogMessage msg)
        {
            if (msg is null)
            {
                throw new System.ArgumentNullException(nameof(msg));
            }

            await _semaphore.WaitAsync();
            {
                if (!_logs.ContainsKey(msg.Instance))
                {
                    _logs.Add(msg.Instance, new List<LogMessage>());
                }

                var log = _logs[msg.Instance];

                lock (log)
                {
                    if (log.Count != 0)
                    {
                        while (log.Last().Id != msg.Id - 1)
                        {
                            if (Monitor.Wait(log, 4000))
                            {
                                break;
                            }
                            else
                            {
                                throw new TimeoutException();
                            }
                        }
                    }
                    log.Add(msg);
                    Monitor.PulseAll(log);
                }
            }
            _semaphore.Release();
        }
    }
}