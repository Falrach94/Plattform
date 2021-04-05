namespace GameServer.Network
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
