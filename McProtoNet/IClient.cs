using McProtoNet.API.Protocol;

namespace McProtoNet
{
    public interface IClient : IDisposable
    {
        bool Connected { get; }


        SessionToken Session { get; }

        event ErrorHandler OnError;
        event DisconnectedHandler OnDisconnected;
        event DisconnectedHandler OnLoginRejected;
        event PacketReceivedHandler OnPacketReceived;
        event PacketSentHandler OnPacketSent;
        event PacketSendHandler OnPacketSend;

        void Start();

        Task SendPacketAsync(Packet packet);

        Task DisconnectAsync();

    }
}
