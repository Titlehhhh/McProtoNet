using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("ShowDialog", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x12)]
public sealed partial class ShowDialogPacket : IServerPacket
{
    public NbtTag Dialog { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteAnonymousNbtTag(Dialog, protocolVersion);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Dialog = reader.ReadAnonymousNbtTag(protocolVersion);

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}