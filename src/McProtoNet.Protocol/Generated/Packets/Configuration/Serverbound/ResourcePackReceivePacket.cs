using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using System;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toServer.resource_pack_receive", PacketPhase.Configuration, PacketDirection.Serverbound)]
[PacketField("Result", "int")]
[PacketField("Uuid", "Guid", Group = "V765_Last", From = 765)]
public sealed partial record ResourcePackReceivePacket(int Result, ResourcePackReceivePacket.V765_LastLayer? V765_Last = null) : IPacket<ResourcePackReceivePacket>, IPacket
{
    public readonly record struct V765_LastLayer(Guid Uuid);
    public static ResourcePackReceivePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResourcePackReceivePacket>(protocolVersion);
        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            var result = reader.ReadVarInt();
            return new ResourcePackReceivePacket(result);
        }

        if (protocolVersion >= 765)
        {
            var uuid = reader.ReadUUID();
            var result = reader.ReadVarInt();
            return new ResourcePackReceivePacket(result, V765_Last: new V765_LastLayer(uuid));
        }

        throw new System.NotSupportedException($"ResourcePackReceivePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResourcePackReceivePacket>(protocolVersion);
        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            writer.WriteVarInt(Result);
            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("ResourcePackReceivePacket", protocolVersion, "V765_Last");
            Guid Uuid = layer.Uuid;
            writer.WriteUUID(Uuid);
            writer.WriteVarInt(Result);
            return;
        }

        throw new System.NotSupportedException($"ResourcePackReceivePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Result");
        writer.WriteNumberValue(Result);
        if (V765_Last is { } v765_Last)
        {
            writer.WritePropertyName("Uuid");
            writer.WriteStringValue(v765_Last.Uuid);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("configuration.toServer.resource_pack_receive", "ResourcePackReceive", PacketPhase.Configuration, PacketDirection.Serverbound, 8);

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
