using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.disconnect", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("ReasonJson", "string", Group = "V764", From = 764, To = 764)]
[PacketField("Reason", "NbtTag", Group = "V765_Last", From = 765)]
public sealed partial record DisconnectPacket(DisconnectPacket.V764Layer? V764 = null, DisconnectPacket.V765_LastLayer? V765_Last = null) : IPacket<DisconnectPacket>
{
    public readonly record struct V764Layer(string ReasonJson);
    public readonly record struct V765_LastLayer(NbtTag Reason);
    public static DisconnectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DisconnectPacket>(protocolVersion);
        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            var reasonJson = reader.ReadString();
            return new DisconnectPacket(V764: new V764Layer(reasonJson));
        }

        if (protocolVersion >= 765)
        {
            var reason = reader.ReadNbtTag(true)!;
            return new DisconnectPacket(V765_Last: new V765_LastLayer(reason));
        }

        throw new System.NotSupportedException($"DisconnectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DisconnectPacket>(protocolVersion);
        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            var layer = V764 ?? throw new WrongLayerException("DisconnectPacket", protocolVersion, "V764");
            string ReasonJson = layer.ReasonJson;
            writer.WriteString(ReasonJson);
            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("DisconnectPacket", protocolVersion, "V765_Last");
            NbtTag Reason = layer.Reason;
            writer.WriteNbt(Reason);
            return;
        }

        throw new System.NotSupportedException($"DisconnectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("configuration.toClient.disconnect", "Disconnect", PacketPhase.Configuration, PacketDirection.Clientbound, 0);

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
