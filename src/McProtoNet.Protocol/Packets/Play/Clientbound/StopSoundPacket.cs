using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("StopSound", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x52)]
[PacketId(751, 754, 0x52)]
[PacketId(755, 756, 0x5D)]
[PacketId(757, 759, 0x5E)]
[PacketId(760, 760, 0x61)]
[PacketId(761, 761, 0x5F)]
[PacketId(762, 763, 0x63)]
[PacketId(764, 764, 0x66)]
[PacketId(765, 765, 0x68)]
[PacketId(766, 767, 0x6A)]
[PacketId(768, 769, 0x71)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x70)]
public sealed partial class StopSoundPacket : IServerPacket
{
    public sbyte Flags { get; set; }
    public int? Source { get; set; }
    public string? Sound { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedByte(Flags);
        if (protocolVersion == 1 || protocolVersion == 3)
        {
            writer.WriteVarInt(Source ?? throw new InvalidOperationException("StopSoundPacket Source field missing."));
        }
        if (protocolVersion == 2 || protocolVersion == 3)
        {
            writer.WriteString(Sound ?? throw new InvalidOperationException("StopSoundPacket Sound field missing."));
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Flags = reader.ReadSignedByte();
        if (protocolVersion == 1 || protocolVersion == 3)
        {
            Source = reader.ReadVarInt();
        }
        else
        {
            Source = null;
        }
        if (protocolVersion == 2 || protocolVersion == 3)
        {
            Sound = reader.ReadString();
        }
        else
        {
            Sound = null;
        }
    }
}