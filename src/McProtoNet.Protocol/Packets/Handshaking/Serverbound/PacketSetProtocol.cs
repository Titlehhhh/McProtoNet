using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Handshaking.Serverbound;

[PacketInfo("SetProtocol", PacketState.Handshaking, PacketDirection.Serverbound)]
public sealed partial class SetProtocolPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)
    };

    public int ProtocolVersion { get; set; }
    public string ServerHost { get; set; } = string.Empty;
    public ushort ServerPort { get; set; }
    public int NextState { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(ProtocolVersion);
                writer.WriteString(ServerHost);
                writer.WriteUnsignedShort(ServerPort);
                writer.WriteVarInt(NextState);
                break;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientHandshakingPacket.SetProtocol), protocolVersion, SupportedVersionsStatic);
           
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                ProtocolVersion = reader.ReadVarInt();
                ServerHost = reader.ReadString();
                ServerPort = reader.ReadUnsignedShort();
                NextState = reader.ReadVarInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientHandshakingPacket.SetProtocol), protocolVersion, SupportedVersionsStatic);
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}