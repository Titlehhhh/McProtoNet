using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("ResourcePackReceive", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x20)]
[PacketId(751, 758, 0x21)]
[PacketId(759, 759, 0x23)]
[PacketId(760, 763, 0x24)]
[PacketId(764, 764, 0x27)]
[PacketId(765, 765, 0x28)]
[PacketId(766, 767, 0x2B)]
[PacketId(768, 768, 0x2D)]
[PacketId(769, 770, 0x2F)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x30)]
public sealed partial class ResourcePackReceivePacket : IClientPacket
{
    public int Result { get; set; }
    public V765_LastFields? V765_Last { get; set; }

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
                var fields = V765_Last ?? throw new InvalidOperationException("ResourcePackReceivePacket 765-last fields missing.");
                writer.WriteUUID(fields.Uuid);
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
                V765_Last = null;
                Result = reader.ReadVarInt();
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                V765_Last = new V765_LastFields
                {
                    Uuid = reader.ReadUUID()
                };
                Result = reader.ReadVarInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ResourcePackReceivePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V765_LastFields
    {
        public Guid Uuid { get; set; }
    }
}