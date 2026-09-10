using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.custom_payload", PacketPhase.Play, PacketDirection.Serverbound)]
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

    public static PacketIdentity Identity => new("play.toServer.custom_payload", "CustomPayload", PacketPhase.Play, PacketDirection.Serverbound, 21);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
