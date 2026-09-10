using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.open_horse_window", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("WindowId", "int")]
[PacketField("NbSlots", "int")]
[PacketField("EntityId", "int")]
public sealed partial record OpenHorseWindowPacket(int WindowId, int NbSlots, int EntityId) : IPacket<OpenHorseWindowPacket>, IPacket
{
    public static OpenHorseWindowPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenHorseWindowPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var windowId = reader.ReadUnsignedByte();
            var nbSlots = reader.ReadVarInt();
            var entityId = reader.ReadSignedInt();
            return new OpenHorseWindowPacket(windowId, nbSlots, entityId);
        }

        if (protocolVersion >= 768)
        {
            var windowId = reader.ReadVarInt();
            var nbSlots = reader.ReadVarInt();
            var entityId = reader.ReadSignedInt();
            return new OpenHorseWindowPacket(windowId, nbSlots, entityId);
        }

        throw new System.NotSupportedException($"OpenHorseWindowPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenHorseWindowPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            writer.WriteUnsignedByte((byte)WindowId);
            writer.WriteVarInt(NbSlots);
            writer.WriteSignedInt(EntityId);
            return;
        }

        if (protocolVersion >= 768)
        {
            writer.WriteVarInt(WindowId);
            writer.WriteVarInt(NbSlots);
            writer.WriteSignedInt(EntityId);
            return;
        }

        throw new System.NotSupportedException($"OpenHorseWindowPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.open_horse_window", "OpenHorseWindow", PacketPhase.Play, PacketDirection.Clientbound, 66);

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
