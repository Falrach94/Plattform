using NetworkUtils.Socket;

namespace GameServer
{
    public class EndpointChangedEventArgs
    {
        public IEndpoint Endpoint { get; private set; }

        public EndpointEventType Type { get; private set; }

        public DisconnectedArgs DisconnectArgs { get; private set; }

        private EndpointChangedEventArgs() { }
        public static EndpointChangedEventArgs NewConnection(IEndpoint endpoint)
        {
            return new EndpointChangedEventArgs()
            {
                Endpoint = endpoint,
                Type = EndpointEventType.Connect
            };
        }
        public static EndpointChangedEventArgs Disconnect(IEndpoint endpoint, DisconnectedArgs args)
        {
            return new EndpointChangedEventArgs()
            {
                Endpoint = endpoint,
                Type = EndpointEventType.Disconnect,
                DisconnectArgs = args
            };
        }

    }
}