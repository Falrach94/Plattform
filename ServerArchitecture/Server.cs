
using LogUtils;
using PatternUtils.Module_Framework;
using ServerKernel;
using ServerKernel.Connections;
using ServerKernel.Network;
using ServerKernel.Control;
using SyncUtils;
using System;
using System.Threading.Tasks;
using ServerKernel.Messaging;

namespace ServerKernel
{

    public class Server
    {
        public IModuleManager ModuleManager { get; } = new ModuleManager();

        private readonly ConnectionModule _connectionModule = new();
        private readonly ControlModule _controlModule;
        private readonly NetworkModule _networkModule = new();
        private readonly MessagingModule _messagingModule = new();

        private IServerControl _control;

        private async Task InitializeFrameworkAsync()
        {
            await ModuleManager.RegisterModuleAsync(_networkModule.Header);
            await ModuleManager.RegisterModuleAsync(_controlModule.Header);
            await ModuleManager.RegisterModuleAsync(_connectionModule.Header);
            await ModuleManager.RegisterModuleAsync(_messagingModule.Header);

            _control = await ModuleManager.GetInterfaceAsync<IServerControl>(null);
        }

        public Server()
        {
            _controlModule = new(ModuleManager);

            InitializeFrameworkAsync().Wait();

        }

        public async Task StartAsync(int port = 6666)
        {
            await _control.ResetAllAsync(port);
        }
        public async Task StopAsync()
        {
            await ModuleManager.StopAllModulesAsync();
        }


    }
}