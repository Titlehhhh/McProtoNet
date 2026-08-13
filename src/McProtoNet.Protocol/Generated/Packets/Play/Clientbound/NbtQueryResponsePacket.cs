using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.nbt_query_response", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("TransactionId", "int")]
[PacketField("Nbt", "NbtTag?")]
public sealed partial record NbtQueryResponsePacket(int TransactionId, NbtTag? Nbt) : IPacket<NbtQueryResponsePacket>, IPacket
{
    public static NbtQueryResponsePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<NbtQueryResponsePacket>(protocolVersion);
        if (protocolVersion <= 763)
        {
            var transactionId = reader.ReadVarInt();
            NbtTag? nbt = null;
            if (reader.ReadBoolean())
                nbt = reader.ReadNbtTag(true)!;
            return new NbtQueryResponsePacket(transactionId, nbt);
        }

        if (protocolVersion >= 764)
        {
            var transactionId = reader.ReadVarInt();
            NbtTag? nbt = null;
            if (reader.ReadBoolean())
                nbt = reader.ReadNbtTag(false)!;
            return new NbtQueryResponsePacket(transactionId, nbt);
        }

        throw new System.NotSupportedException($"NbtQueryResponsePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<NbtQueryResponsePacket>(protocolVersion);
        if (protocolVersion <= 763)
        {
            writer.WriteVarInt(TransactionId);
            writer.WriteBoolean(Nbt is not null);
            if (Nbt is { } nbtValue)
                writer.WriteNbt(nbtValue, true);
            return;
        }

        if (protocolVersion >= 764)
        {
            writer.WriteVarInt(TransactionId);
            writer.WriteBoolean(Nbt is not null);
            if (Nbt is { } nbtValue)
                writer.WriteNbt(nbtValue);
            return;
        }

        throw new System.NotSupportedException($"NbtQueryResponsePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.nbt_query_response", "NbtQueryResponse", PacketPhase.Play, PacketDirection.Clientbound, 55);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x54;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x54;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            id = 0x5F;
            return true;
        }

        if (protocolVersion >= 757 && protocolVersion <= 758)
        {
            id = 0x60;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x61;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x64;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x62;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x66;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x69;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x6B;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x6E;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x75;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x74;
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
