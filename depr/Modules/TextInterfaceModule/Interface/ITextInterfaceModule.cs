namespace TextInterfaceModule.Interface
{
    public interface ITextInterfaceModule
    {
        ICommandInterface CommandInterface { get; }
        ITextInterfaceControl InterfaceControl { get; }
    }
}
