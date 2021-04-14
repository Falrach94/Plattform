using MessageUtils;
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
        public Task HandleMessageErrorAsync(MessageProcessingError error)
        {
            return Task.CompletedTask;
        }
    }
}
