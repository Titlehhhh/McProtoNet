using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("Disconnect", PacketState.Login, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0x00)]
public sealed partial class DisconnectPacket : IServerPacket
{
    public string Name { get; set; } = string.Empty;

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteString(Name);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Name = reader.ReadString();
}