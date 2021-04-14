using MessageUtils;
using System.Threading.Tasks;

namespace ServerKernel.Messaging
{
    public interface IMessageErrorProtocol
    {
        Task HandleMessageErrorAsync(MessageProcessingError error);
    }
}