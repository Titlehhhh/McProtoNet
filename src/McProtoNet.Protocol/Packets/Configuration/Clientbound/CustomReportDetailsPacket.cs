using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("CustomReportDetails", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
[PacketId(767, MinecraftVersion.LatestProtocol, 0x0F)]
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
        var details = new DetailsEntry[count];
        for (int i = 0; i < count; i++)
        {
            details[i] = new DetailsEntry
            {
                Key = reader.ReadString(),
                Value = reader.ReadString()
            };
        }
        Details = details;
    }

    public struct DetailsEntry
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}