using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ShowDialog", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x85)]
public sealed partial class ShowDialogPacket : IServerPacket
{
    public RegistryEntryHolder<NbtTag> Dialog { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteRegistryEntryHolder<NbtTag>(Dialog, protocolVersion);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Dialog = reader.ReadRegistryEntryHolder<NbtTag>(protocolVersion);
}