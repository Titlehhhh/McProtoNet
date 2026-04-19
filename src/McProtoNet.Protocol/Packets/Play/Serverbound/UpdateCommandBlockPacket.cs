using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("UpdateCommandBlock", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x25)]
[PacketId(751, 758, 0x26)]
[PacketId(759, 759, 0x28)]
[PacketId(760, 763, 0x29)]
[PacketId(764, 764, 0x2C)]
[PacketId(765, 765, 0x2D)]
[PacketId(766, 767, 0x30)]
[PacketId(768, 768, 0x32)]
[PacketId(769, 770, 0x34)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x35)]
public sealed partial class UpdateCommandBlockPacket : IClientPacket
{
    public Position Location { get; set; } = default!;
    public string Command { get; set; } = default!;
    public int Mode { get; set; }
    public byte Flags { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            writer.WriteType(Location, protocolVersion);
            writer.WriteString(Command);
            writer.WriteVarInt(Mode);
            writer.WriteUnsignedByte(Flags);
        }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Location = reader.ReadType<Position>(protocolVersion);
            Command = reader.ReadString();
            Mode = reader.ReadVarInt();
            Flags = reader.ReadUnsignedByte();
        }
}