using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.select_known_packs", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Packs", "KnownPack[]")]
public sealed partial record SelectKnownPacksPacket(KnownPack[] Packs) : IPacket<SelectKnownPacksPacket>, IPacket
{
    public static SelectKnownPacksPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SelectKnownPacksPacket>(protocolVersion);
        int packsCount = reader.ReadVarInt();
        var packs = new KnownPack[packsCount];
        for (int i = 0; i < packs.Length; i++)
            packs[i] = reader.ReadType<KnownPack>(protocolVersion);
        return new SelectKnownPacksPacket(packs);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SelectKnownPacksPacket>(protocolVersion);
        writer.WriteVarInt(Packs.Length);
        foreach (var packsItem in Packs)
            writer.WriteType<KnownPack>(packsItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("configuration.toClient.select_known_packs", "SelectKnownPacks", PacketPhase.Configuration, PacketDirection.Clientbound, 14);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 776)
        {
            id = 0x0E;
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
