using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("StoreCookie", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(766, 767, 0x6B)]
[PacketId(768, 769, 0x72)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x71)]
public sealed partial class StoreCookiePacket : IServerPacket
{
    public string Key { get; set; }
    public byte[] Value { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Key);
        writer.WriteVarInt(Value.Length);
        writer.WriteBuffer(Value);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Key = reader.ReadString();
        int len = reader.ReadVarInt();
        Value = reader.ReadBuffer(len);
    }
}