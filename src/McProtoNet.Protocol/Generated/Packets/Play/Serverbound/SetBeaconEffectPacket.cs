using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toServer.set_beacon_effect", "SetBeaconEffect", PacketPhase.Play, PacketDirection.Serverbound, 48);

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
