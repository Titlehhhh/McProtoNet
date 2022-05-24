using McProtoNet.API.Protocol;

namespace McProtoNet
{
    public interface IClient : IDisposable
    {
        bool Connected { get; }


        event ErrorHandler OnError;
        event DisconnectedHandler OnDisconnected;
        event DisconnectedHandler OnLoginRejected;
        event PacketReceivedHandler OnPacketReceived;
        event PacketSentHandler OnPacketSent;
        event PacketSendHandler OnPacketSend;

        Task SendPacketAsync(Packet packet);

        Task DisconnectAsync();

    }
}
