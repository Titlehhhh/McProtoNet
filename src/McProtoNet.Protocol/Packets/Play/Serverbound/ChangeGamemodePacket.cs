using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("ChangeGamemode", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x04)]
public sealed partial class ChangeGamemodePacket : IClientPacket
{
    public int Mode { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(Mode);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Mode = reader.ReadVarInt();
}