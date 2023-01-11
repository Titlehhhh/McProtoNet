using McProtoNet.Core;
using McProtoNet.Core.Protocol;

namespace McProtoNet
{

    public delegate void PacketReceivedHandler(PacketListener sender, MinecraftPacket packet);
    public delegate void OnErrorHandler(PacketListener sender, Exception exception);
    
    
    
    public delegate void ClientPacketReceivedHandler<TPack>(IClient<TPack> client, MinecraftPacket<TPack> packet) where TPack : IProtocol, new();
    public delegate void ClientDisconnected<TProto>(IClient<TProto> client, Exception? error) where TProto : IProtocol, new();

    public delegate void ServerOnErrorHandler<TProto>(Server<TProto> server, Exception exception) where TProto : IProtocol, new();
    public delegate void ServerStartedHandler<TProto>(Server<TProto> server) where TProto : IProtocol, new();
    public delegate void ClientConnectedHandler<TProto>(IClient<TProto> client) where TProto : IProtocol, new();
    public delegate void ServerStopedHandler<TProto>(Server<TProto> server) where TProto : IProtocol, new();


}
