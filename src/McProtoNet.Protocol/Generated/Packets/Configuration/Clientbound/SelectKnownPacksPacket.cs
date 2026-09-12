using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Packs");
        writer.WriteStartArray();
        foreach (var item0 in Packs)
        {
            item0.WriteJson(writer);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("configuration.toClient.select_known_packs", "SelectKnownPacks", PacketPhase.Configuration, PacketDirection.Clientbound, 15);

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
