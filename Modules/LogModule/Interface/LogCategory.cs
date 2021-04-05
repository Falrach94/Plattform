using LogUtils;

namespace LogModules.Interface
{
    public struct LogCategory
    {
        public LogCategory(string name, LogLevel minLevel)
        {
            Name = name;
            MinLevel = minLevel;
        }

        public string Name { get; }
        public LogLevel MinLevel { get; }
    }
}
