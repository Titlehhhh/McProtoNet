using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[PacketInfo("ResourcePackReceive", PacketState.Configuration, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x05)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x06)]
public sealed partial class ResourcePackReceivePacket : IPacket
{
    public int Result { get; set; }
    public Guid? Uuid { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                writer.WriteVarInt(Result);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteUUID(Uuid ?? throw new InvalidOperationException("ResourcePackReceivePacket 765-last Uuid fields missing."));
                writer.WriteVarInt(Result);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ResourcePackReceivePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                Result = reader.ReadVarInt();
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                Uuid = reader.ReadUUID();
                Result = reader.ReadVarInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ResourcePackReceivePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}