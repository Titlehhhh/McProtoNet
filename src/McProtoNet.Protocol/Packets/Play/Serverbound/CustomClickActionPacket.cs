using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[PacketInfo("CustomClickAction", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x41)]
public sealed partial class CustomClickActionPacket : IPacket
{
    public string Id { get; set; } = "";
    public NbtTag? Nbt { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteString(Id);
                writer.WriteAnonOptionalNbtTag(Nbt, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CustomClickActionPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                Id = reader.ReadString();
                Nbt = reader.ReadAnonOptionalNbtTag(protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CustomClickActionPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}