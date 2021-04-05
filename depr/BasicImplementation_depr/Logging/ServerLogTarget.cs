using NLog;
using NLog.Targets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace GameServer
{
    [Target("ServerLog")]
    public sealed class ServerLogTarget : TargetWithLayout
    {
        private readonly object _mutex = new object();
        private int _nextId = 0;
    //    private static readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();
        public static Stream Out { get; set; }

        public ServerLogTarget()
        {
        }

        protected override void Write(LogEventInfo logEvent)
        {
            int id;
            lock (_mutex)
            {
                id = _nextId++;
            }
            //   Task.Run(() => {
            if (Out != null)
            {
                lock (Out)
                {
                    Out.Position = Out.Length;

                //    _binaryFormatter.Serialize(Out, new LogMessage(logEvent, id));

                    Monitor.PulseAll(Out);
                }
            }
            //     });
        }
    }
}
