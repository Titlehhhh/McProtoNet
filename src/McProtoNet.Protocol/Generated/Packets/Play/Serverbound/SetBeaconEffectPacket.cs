using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.set_beacon_effect", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("PrimaryEffect", "int?")]
[PacketField("SecondaryEffect", "int?")]
public sealed partial record SetBeaconEffectPacket(int? PrimaryEffect, int? SecondaryEffect) : IPacket<SetBeaconEffectPacket>, IPacket
{
    public static SetBeaconEffectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetBeaconEffectPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var primaryEffect = reader.ReadVarInt();
            var secondaryEffect = reader.ReadVarInt();
            return new SetBeaconEffectPacket(primaryEffect, secondaryEffect);
        }

        if (protocolVersion >= 759)
        {
            int? primaryEffect = null;
            if (reader.ReadBoolean())
                primaryEffect = reader.ReadVarInt();
            int? secondaryEffect = null;
            if (reader.ReadBoolean())
                secondaryEffect = reader.ReadVarInt();
            return new SetBeaconEffectPacket(primaryEffect, secondaryEffect);
        }

        throw new System.NotSupportedException($"SetBeaconEffectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetBeaconEffectPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            writer.WriteVarInt((PrimaryEffect ?? throw new System.InvalidOperationException("PrimaryEffect is required at this protocol version.")));
            writer.WriteVarInt((SecondaryEffect ?? throw new System.InvalidOperationException("SecondaryEffect is required at this protocol version.")));
            return;
        }

        if (protocolVersion >= 759)
        {
            writer.WriteBoolean(PrimaryEffect is not null);
            if (PrimaryEffect is { } primaryEffectValue)
                writer.WriteVarInt(primaryEffectValue);
            writer.WriteBoolean(SecondaryEffect is not null);
            if (SecondaryEffect is { } secondaryEffectValue)
                writer.WriteVarInt(secondaryEffectValue);
            return;
        }

        throw new System.NotSupportedException($"SetBeaconEffectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.set_beacon_effect", "SetBeaconEffect", PacketPhase.Play, PacketDirection.Serverbound, 43);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x23;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x24;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x26;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x27;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x2A;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x32;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 772)
        {
            id = 0x33;
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
