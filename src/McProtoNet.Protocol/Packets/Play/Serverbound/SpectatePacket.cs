using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("Spectate", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2C)]
[PacketId(751, 758, 0x2D)]
[PacketId(759, 759, 0x2F)]
[PacketId(760, 763, 0x30)]
[PacketId(764, 764, 0x33)]
[PacketId(765, 765, 0x34)]
[PacketId(766, 767, 0x37)]
[PacketId(768, 768, 0x39)]
[PacketId(769, 769, 0x3B)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x3D)]
public sealed partial class SpectatePacket : IClientPacket
{
    public UUID Target { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteUUID(Target);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Target = reader.ReadUUID();
    }
}