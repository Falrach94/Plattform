using LogUtils;
using System.Drawing;

namespace ServerGUI
{
    public class CategoryConfiguration
    {
        public CategoryConfiguration(bool show, LogLevel minLevel, string name)
            : this(show, minLevel, name, default)
        {
        }
        public CategoryConfiguration(bool show, LogLevel minLevel, string name, Color color)
        {
            Show = show;
            MinLevel = minLevel;
            Name = name;
            Color = color;
        }

        public bool Show { get; set; }
        public LogLevel MinLevel { get; set; }
        public string Name { get; }

        public Color Color { get; }
    }
}
