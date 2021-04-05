using LogUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class LogManager : LogUtils.LogManager
    {
        private static LogManager _instance;

        private LogManager() : base("Server"){ }

        public static LogManager GetInstance()
        {
            if(_instance == null)
            {
                _instance = new LogManager();
            }
            return _instance;
        }
        public new static Logger GetLogger(string name)
        {
            return ((LogUtils.LogManager)GetInstance()).GetLogger(name);
        }
    }
}
