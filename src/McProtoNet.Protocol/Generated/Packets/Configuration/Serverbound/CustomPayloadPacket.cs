using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toServer.custom_payload", PacketPhase.Configuration, PacketDirection.Serverbound)]
[PacketField("Channel", "string")]
[PacketField("Data", "byte[]")]
public sealed partial record CustomPayloadPacket(string Channel, byte[] Data) : IPacket<CustomPayloadPacket>, IPacket
{
    public static CustomPayloadPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CustomPayloadPacket>(protocolVersion);
        var channel = reader.ReadString();
        var data = reader.ReadRestBytes();
        return new CustomPayloadPacket(channel, data);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CustomPayloadPacket>(protocolVersion);
        writer.WriteString(Channel);
        writer.WriteRestBytes(Data);
    }

    public static PacketIdentity Identity => new("configuration.toServer.custom_payload", "CustomPayload", PacketPhase.Configuration, PacketDirection.Serverbound, 2);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x01;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 772)
        {
            id = 0x02;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
