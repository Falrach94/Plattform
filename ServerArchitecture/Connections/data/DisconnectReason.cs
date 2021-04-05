namespace ServerKernel.Data_Objects
{
    public enum DisconnectReason
    {
        Kick,
        Timeout,
        ProtocolViolation,
        ServerClosed,
        InternalError
    }
}
