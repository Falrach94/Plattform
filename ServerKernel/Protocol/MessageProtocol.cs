using MessageUtils;
using MessageUtils.Messenger;
using ServerKernel.Connections.Manager;
using ServerKernel.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKernel.Protocol
{
    public class MessageProtocol : IMessageErrorProtocol
    {
        public IConnectionControl ConnectionControl { get; set; }
        public BroadcastMessenger Messenger { get; set; }

        public Task HandleMessageErrorAsync(MessageProcessingError error)
        {
            return Task.CompletedTask;
        }
    }
}
