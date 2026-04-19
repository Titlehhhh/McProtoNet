using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Title", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x4F)]
[PacketId(751, 754, 0x4F)]
public sealed partial class TitlePacket : IServerPacket
{
    public int Action { get; set; }
    public string Text { get; set; }
    public int FadeIn { get; set; }
    public int Stay { get; set; }
    public int FadeOut { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Action);
        switch (Action)
        {
            case 0:
            case 1:
            case 2:
                writer.WriteString(Text);
                break;
            case 3:
                writer.WriteSignedInt(FadeIn);
                writer.WriteSignedInt(Stay);
                writer.WriteSignedInt(FadeOut);
                break;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Action = reader.ReadVarInt();
        switch (Action)
        {
            case 0:
            case 1:
            case 2:
                Text = reader.ReadString();
                break;
            case 3:
                FadeIn = reader.ReadSignedInt();
                Stay = reader.ReadSignedInt();
                FadeOut = reader.ReadSignedInt();
                break;
        }
    }
}