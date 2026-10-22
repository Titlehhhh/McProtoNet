using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;
using System;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.add_resource_pack", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Uuid", "Guid")]
[PacketField("Url", "string")]
[PacketField("Hash", "string")]
[PacketField("Forced", "bool")]
[PacketField("PromptMessage", "NbtTag?")]
public sealed partial record AddResourcePackPacket(Guid Uuid, string Url, string Hash, bool Forced, NbtTag? PromptMessage) : IPacket<AddResourcePackPacket>, IPacket
{
    public static AddResourcePackPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AddResourcePackPacket>(protocolVersion);
        var uuid = reader.ReadUUID();
        var url = reader.ReadString();
        var hash = reader.ReadString();
        var forced = reader.ReadBoolean();
        NbtTag? promptMessage = null;
        if (reader.ReadBoolean())
            promptMessage = reader.ReadNbtTag(false)!;
        return new AddResourcePackPacket(uuid, url, hash, forced, promptMessage);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AddResourcePackPacket>(protocolVersion);
        writer.WriteUUID(Uuid);
        writer.WriteString(Url);
        writer.WriteString(Hash);
        writer.WriteBoolean(Forced);
        writer.WriteBoolean(PromptMessage is not null);
        if (PromptMessage is { } promptMessageValue)
            writer.WriteNbt(promptMessageValue);
    }

    public static PacketIdentity Identity => new("configuration.toClient.add_resource_pack", "AddResourcePack", PacketPhase.Configuration, PacketDirection.Clientbound, 0);

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
