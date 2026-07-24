using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Handshaking.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class SetProtocolPacket : IProtocolType<SetProtocolPacket>
{
    public int ProtocolVersion { get; }
    public string ServerHost { get; }
    public int ServerPort { get; }
    public int NextState { get; }

    public SetProtocolPacket(int protocolVersion, string serverHost, int serverPort, int nextState)
    {
        ProtocolVersion = protocolVersion;
        ServerHost = serverHost;
        ServerPort = serverPort;
        NextState = nextState;
    }

    public static SetProtocolPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetProtocolPacket>(protocolVersion);
        var protocolVersion_ = reader.ReadVarInt();
        var serverHost = reader.ReadString();
        var serverPort = reader.ReadUnsignedShort();
        var nextState = reader.ReadVarInt();
        return new SetProtocolPacket(protocolVersion_, serverHost, serverPort, nextState);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetProtocolPacket>(protocolVersion);
        writer.WriteVarInt(ProtocolVersion);
        writer.WriteString(ServerHost);
        writer.WriteUnsignedShort((ushort)ServerPort);
        writer.WriteVarInt(NextState);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 772)
            return 0x00;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
