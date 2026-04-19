using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetTitleTime", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 756, 0x5A)]
[PacketId(757, 759, 0x5B)]
[PacketId(760, 760, 0x5E)]
[PacketId(761, 761, 0x5C)]
[PacketId(762, 763, 0x60)]
[PacketId(764, 764, 0x62)]
[PacketId(765, 765, 0x64)]
[PacketId(766, 767, 0x66)]
[PacketId(768, 769, 0x6D)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x6C)]
public sealed partial class SetTitleTimePacket : IServerPacket
{
    public int FadeIn { get; set; }
    public int Stay { get; set; }
    public int FadeOut { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedInt(FadeIn);
        writer.WriteSignedInt(Stay);
        writer.WriteSignedInt(FadeOut);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        FadeIn = reader.ReadSignedInt();
        Stay = reader.ReadSignedInt();
        FadeOut = reader.ReadSignedInt();
    }
}