using GameServer.Data_Objects;
using LogUtils;
using MessageUtilities;
using MessageUtility.MessageDictionary;
using ServerImplementation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameServer
{
    public class FrameworkManager : IModuleFramework, IModuleControl
    {
        #region fields

        private readonly Logger log = Logging.LogManager.GetLogger("Framework");

        #endregion

        #region properties
        public Dictionary<string, IModule> Modules { get; } = new Dictionary<string, IModule>();
        public IMessenger Messenger { get; set; }
        public IServer Server { get; set; }
        public IReadOnlyCollection<IModule> LoadedModules => Modules.Values;
        IReadOnlyCollection<IModule> IModuleFramework.Modules => new HashSet<IModule>(Modules.Values);

        #endregion

        public void LoadModule(IModule module)
        {
            module.Logic.Messenger = Messenger;
            module.MessageHandler.Messenger = Messenger;
            module.Server = Server;
            module.Logic.Server = Server;
            module.MessageHandler.Server = Server;

            foreach (var m in Modules.Values)
            {
                m.GreetModule(module);
                module.GreetModule(m);
            }

            Modules.Add(module.Name, module);

        }

        public Task DistributeMessage(IConnection sender, Message msg)
        {
            if (!Modules.ContainsKey(msg.Module))
            {
                throw new Exception("Message targeted at unrecognized module.");
            }

            return Modules[msg.Module].MessageHandler.ProcessMessage(sender, msg);
        }

        public async Task HandleDisconnect(IConnection client)
        {
            using (var token = await client.StateMutex.LockAsync(Const.DEADLOCK_TIMEOUT))
            {
                while (client.State == ConnectionState.Connecting)
                {
                    await client.StateMutex.WaitAsync(token, Const.DEADLOCK_TIMEOUT);
                }
            }

            List<Task> tasks = new List<Task>();
            foreach (var m in Modules.Values)
            {
                tasks.Add(m.Logic.HandleDisconnect(client));
            }
            await Task.WhenAll(tasks);
        }

        public Task InitializeModules()
        {
            foreach (var m in Modules.Values)
            {
                log.Debug("initializing " + m.Name);
                m.Logic.Initialize();
            }

            log.Debug("all loaded modules initialized");
            MessageDictionary.GetInstance().CreatePDF("messages.txt");
            return Task.CompletedTask;
        }

        public void LoadState()
        {
            throw new NotImplementedException();
        }

        public async Task Reset()
        {
            foreach (var m in Modules.Values)
            {
                log.Debug("resetting " + m.Name);
                await m.Logic.Reset();
            }
            log.Debug("all modules reset");
        }

        public void SaveState()
        {
            throw new NotImplementedException();
        }

        public async Task ShutDown()
        {
            foreach (var m in Modules.Values)
            {
                log.Debug("shutting down " + m.Name);
                await m.Logic.ShutDown();
            }
            log.Debug("all modules shut down");
        }

        public async Task SynchronizeClient(IConnection client)
        {
            foreach (var m in Modules.Values)
            {
                if (client.ModuleDic[m.Name])
                {
                    await m.Logic.SynchronizeConnection(client);
                }
            }
        }

    }
}