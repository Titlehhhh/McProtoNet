using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.spectate", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Target", "Guid")]
public sealed partial record SpectatePacket(Guid Target) : IPacket<SpectatePacket>, IPacket
{
    public static SpectatePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpectatePacket>(protocolVersion);
        var target = reader.ReadUUID();
        return new SpectatePacket(target);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpectatePacket>(protocolVersion);
        writer.WriteUUID(Target);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Target");
        writer.WriteStringValue(Target);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.spectate", "Spectate", PacketPhase.Play, PacketDirection.Serverbound, 53);

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
