using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.kick_disconnect", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("ReasonJson", "string", Group = "VUntil764", To = 764)]
[PacketField("Reason", "NbtTag", Group = "V765_Last", From = 765)]
public sealed partial record KickDisconnectPacket(KickDisconnectPacket.VUntil764Layer? VUntil764 = null, KickDisconnectPacket.V765_LastLayer? V765_Last = null) : IPacket<KickDisconnectPacket>, IPacket
{
    public readonly record struct VUntil764Layer(string ReasonJson);
    public readonly record struct V765_LastLayer(NbtTag Reason);
    public static KickDisconnectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<KickDisconnectPacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var reasonJson = reader.ReadString();
            return new KickDisconnectPacket(VUntil764: new VUntil764Layer(reasonJson));
        }

        if (protocolVersion >= 765)
        {
            var reason = reader.ReadNbtTag(false)!;
            return new KickDisconnectPacket(V765_Last: new V765_LastLayer(reason));
        }

        throw new System.NotSupportedException($"KickDisconnectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<KickDisconnectPacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var layer = VUntil764 ?? throw new WrongLayerException("KickDisconnectPacket", protocolVersion, "VUntil764");
            string ReasonJson = layer.ReasonJson;
            writer.WriteString(ReasonJson);
            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("KickDisconnectPacket", protocolVersion, "V765_Last");
            NbtTag Reason = layer.Reason;
            writer.WriteNbt(Reason);
            return;
        }

        throw new System.NotSupportedException($"KickDisconnectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.kick_disconnect", "KickDisconnect", PacketPhase.Play, PacketDirection.Clientbound, 51);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x17;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x17;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 769)
        {
            id = 0x1D;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 776)
        {
            id = 0x20;
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
