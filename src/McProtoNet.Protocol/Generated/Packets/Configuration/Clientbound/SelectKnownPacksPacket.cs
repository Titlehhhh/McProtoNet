using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class SelectKnownPacksPacket : IProtocolType<SelectKnownPacksPacket>
{
    public KnownPack[] Packs { get; }

    public SelectKnownPacksPacket(KnownPack[] packs)
    {
        Packs = packs;
    }

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

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x0E;
        if (protocolVersion >= 767 && protocolVersion <= 770)
            return 0x0E;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x0E;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
