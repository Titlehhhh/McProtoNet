using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("RecipeBookRemove", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[PacketId(768, 769, 0x45)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x44)]
public sealed partial class RecipeBookRemovePacket : IServerPacket
{
    public int[] RecipeIds { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarIntArray(RecipeIds);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => RecipeIds = reader.ReadVarIntArray();
}