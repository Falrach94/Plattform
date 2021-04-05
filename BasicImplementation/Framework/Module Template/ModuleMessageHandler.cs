using GameServer;
using GameServer.Data_Objects;
using LogUtils;
using MessageUtilities;
using MessageUtility.MessageDictionary;
using ServerImplementation.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerImplementation
{
    public delegate Task MessageHandlerDelegate(IConnection connection, Message message);

    public abstract class ModuleMessageHandler<TData, TLogic> : IManagedModuleMessageHandler<TLogic, TData>
        where TData : IModuleData
        where TLogic : IModuleLogic
    {
        private static readonly MessageDictionary Messages = MessageDictionary.GetInstance();

        private string _name;

        protected Logger log;
        public string Name { get => _name; set { _name = value; MessageRegistration(); } }
        public string LogName { set => log = Logging.LogManager.GetLogger(value); }
        public TData Data { get; set; }
        public TLogic Logic { get; set; }

        public IMessenger Messenger { get; set; }
        public IServer Server { get; set; }

        IModuleLogic IModuleMessageHandler.Logic => Logic;
        IModuleData IModuleMessageHandler.Data => Data;


        private readonly Dictionary<string, MessageHandlerDelegate> _handlerDic = new Dictionary<string, MessageHandlerDelegate>();

        protected abstract void MessageRegistration();

        protected void RegisterMessage(string type, MessageHandlerDelegate handler, Type dataType, params string[] desc)
        {
            if (handler == null)
            {
                throw new ArgumentNullException();
            }

            _handlerDic.Add(type, handler);

            Messages.AddIngoingMessage(Name, type, dataType, desc);
        }

        public Task ProcessMessage(IConnection sender, Message msg)
        {
            var type = msg.Type;

            log.Info("received '" + type + "-message");

            if (!_handlerDic.ContainsKey(type))
            {
                log.Warn("unimplemented message type: " + type);
                return Task.CompletedTask;
            }
            else
            {
                return _handlerDic[type](sender, msg);
            }
        }
    }
}
