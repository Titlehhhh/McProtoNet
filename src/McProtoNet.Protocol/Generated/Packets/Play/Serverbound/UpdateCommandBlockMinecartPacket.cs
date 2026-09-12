using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.update_command_block_minecart", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("EntityId", "int")]
[PacketField("Command", "string")]
[PacketField("TrackOutput", "bool")]
public sealed partial record UpdateCommandBlockMinecartPacket(int EntityId, string Command, bool TrackOutput) : IPacket<UpdateCommandBlockMinecartPacket>, IPacket
{
    public static UpdateCommandBlockMinecartPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateCommandBlockMinecartPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var command = reader.ReadString();
        var trackOutput = reader.ReadBoolean();
        return new UpdateCommandBlockMinecartPacket(entityId, command, trackOutput);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateCommandBlockMinecartPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteString(Command);
        writer.WriteBoolean(TrackOutput);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
        writer.WritePropertyName("Command");
        writer.WriteStringValue(Command);
        writer.WritePropertyName("TrackOutput");
        writer.WriteBooleanValue(TrackOutput);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.update_command_block_minecart", "UpdateCommandBlockMinecart", PacketPhase.Play, PacketDirection.Serverbound, 62);

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
