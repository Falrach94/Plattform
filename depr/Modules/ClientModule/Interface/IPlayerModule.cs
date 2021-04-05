using TextInterfaceModule.Interface;

namespace PlayerModule.Interface
{
    public interface IPlayerModule
    {

        IPlayerStorage PlayerStorage { get; }
    }
}
