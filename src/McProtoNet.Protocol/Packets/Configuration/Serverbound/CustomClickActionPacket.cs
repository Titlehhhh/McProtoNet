using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("CustomClickAction", PacketState.Configuration, PacketDirection.Serverbound)]
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x08)]
public sealed partial class CustomClickActionPacket : IClientPacket
{
    public string Id { get; set; }
    public NbtTag? Nbt { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Id);
        writer.WriteAnonOptionalNbtTag(Nbt, protocolVersion);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Id = reader.ReadString();
        Nbt = reader.ReadAnonOptionalNbtTag(protocolVersion);
    }
}