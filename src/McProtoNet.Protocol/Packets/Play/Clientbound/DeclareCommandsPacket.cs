using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("DeclareCommands", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x11)]
[PacketId(751, 754, 0x10)]
[PacketId(755, 758, 0x12)]
[PacketId(759, 760, 0x0F)]
[PacketId(761, 761, 0x0E)]
[PacketId(762, 763, 0x10)]
[PacketId(764, 769, 0x11)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x10)]
public sealed partial class DeclareCommandsPacket : IServerPacket
{
    public CommandNode[] Nodes { get; set; }
    public int RootIndex { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteArray<CommandNode>(Nodes, protocolVersion);
        writer.WriteVarInt(RootIndex);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Nodes = reader.ReadArray<CommandNode>(LengthFormat.VarInt, protocolVersion);
        RootIndex = reader.ReadVarInt();
    }
}