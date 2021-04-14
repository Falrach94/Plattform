using MessageUtilities;
using MessageUtils.Message_Socket;
using MessageUtils.MessageHandler;
using MessageUtils.Messenger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtils.Endpoint;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using SererTests.TestUtilities;
using ServerKernel.Messaging;
using ServerTests.Mocks;
using SyncUtils;
using SyncUtils.Barrier;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTests.Messaging
{

    [TestClass]
    public class MessagingModuleTest
    {
        const string MSG_TYPE1 = "type 1";

        class MockMessageHandler : MessageHandler
        {
            public Func<MockMessageHandler, object, Message, Task> MessageHandlerCallback { get; set; }

            public MockMessageHandler(string module) : base(module)
            {
            }

            protected override void MessageRegistration()
            {
                RegisterMessage(MSG_TYPE1, HandlerMessage1, typeof(string));
            }

            private Task HandlerMessage1(object sender, Message message)
            {
                if (MessageHandlerCallback != null)
                {
                    return MessageHandlerCallback(this, sender, message);
                }
                return Task.CompletedTask;
            }
        }

        class MockModule : ModuleControl
        {
            public MockMessageHandler Handler { get; private set; } 
            public MockModule(string module) : base(module, PatternUtils.Version.Create(1), module)
            {
            }

            protected override void Initialize(object[] data)
            {
                Handler = new((string)data[0]);
            }

            protected override void DefineModule(ModuleBuilder builder)
            {
                builder.SetManagedInterfaces(new ManagedInterfaceWrapper<IMessageHandler>(Handler, PatternUtils.Version.Create(1)));
            }

            protected override Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken providerLockToken)
            {
                return Task.CompletedTask;
            }

            protected override Task ResetAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task StartAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task StopAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task UninitializeAsync()
            {
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        public void TestBasics()
        {
            const string MODULE_NAME = "module";
            const int MODULE_COUNT = 10;
            const int EP_COUNT = 10;
            const string MSG = "MESSAGE";

            ModuleManager manager = new();
            MockBaseModule baseModule = new();
            MessagingModule messageModule = new();

            List<MockModule> handlerModules = new();

            int i;
            for(i = 0; i < MODULE_COUNT; i++)
            {
                handlerModules.Add(new(MODULE_NAME + i));
            }
                

            baseModule.MockProtocol.DelayTime = 0;
            for (i = 0; i < MODULE_COUNT / 2; i++)
            {
                TestUtils.AssertTask(manager.RegisterModuleAsync(handlerModules[i].Header));
            }

            TestUtils.AssertTask(manager.RegisterModuleAsync(baseModule.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(messageModule.Header));

            for(; i < MODULE_COUNT; i++)
            {
                TestUtils.AssertTask(manager.RegisterModuleAsync(handlerModules[i].Header));
            }


            BroadcastMessenger messenger = TestUtils.AssertTask(manager.GetInterfaceAsync<BroadcastMessenger>(null));

            ConcurrentBag<MockEndpoint> endpoints = new();
            TestUtils.AssertTask(AsyncUtilities.RepeatAsync(() =>
            {
                var ep = new MockEndpoint();
                baseModule.ConnectEndpoint(ep);
                endpoints.Add(ep);
                return Task.CompletedTask;
            }, EP_COUNT));

            ConcurrentDictionary<Tuple<MockMessageHandler, MockEndpoint>, Message> msgDic = new();

            baseModule.MockProtocol.MessageErrorHandler = async error =>
            {
                Assert.IsFalse(true, "should never be reached!");
            };


            AsyncBarrier barrier = new(EP_COUNT*MODULE_COUNT, false);

            AsyncInt c = 0;
            //send a message to each handler via each endpoint
            foreach(var module in handlerModules)
            {
                module.Handler.MessageHandlerCallback = async (handler, ep, msg) =>
                {
                    Message expectedMsg;
                    Assert.IsTrue(msgDic.TryGetValue((handler, (MockEndpoint)ep).ToTuple(), out expectedMsg));
                    Assert.AreEqual(expectedMsg, msg);
                    await barrier.SignalAsync();
                };

                AsyncUtilities.ForEachAsync(async ep =>
                {
                    Message msg = new Message(module.Handler.Module, MSG_TYPE1, MSG + c.PostIncrement());

                    Assert.IsTrue(msgDic.TryAdd((module.Handler, ep).ToTuple(), msg));

                    await ep.SendAsync(msg);

                }, endpoints);

            }

            TestUtils.AssertTask(barrier.WaitAsync(), 1000000);


        }


    }
}
