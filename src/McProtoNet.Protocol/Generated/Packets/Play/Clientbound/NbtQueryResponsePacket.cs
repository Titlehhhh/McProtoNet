using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
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

    public static PacketIdentity Identity => new("play.toClient.nbt_query_response", "NbtQueryResponse", PacketPhase.Play, PacketDirection.Clientbound, 64);

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
