using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CustomReportDetails", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
[PacketId(767, 767, 0x7A)]
[PacketId(768, MinecraftVersion.LatestProtocol, 0x81)]
public sealed partial class CustomReportDetailsPacket : IServerPacket
{
    public DetailsEntry[] Details { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Details.Length);
        foreach (var entry in Details)
        {
            writer.WriteString(entry.Key);
            writer.WriteString(entry.Value);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        var array = new DetailsEntry[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = new DetailsEntry
            {
                Key = reader.ReadString(),
                Value = reader.ReadString()
            };
        }
        Details = array;
    }

    public struct DetailsEntry
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}