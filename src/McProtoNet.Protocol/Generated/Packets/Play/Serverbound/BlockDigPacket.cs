using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.block_dig", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Status", "int")]
[PacketField("Location", "Position")]
[PacketField("Face", "int")]
[PacketField("Sequence", "int", Group = "V759_Last", From = 759)]
public sealed partial record BlockDigPacket(int Status, Position Location, int Face, BlockDigPacket.V759_LastLayer? V759_Last = null) : IPacket<BlockDigPacket>, IPacket
{
    public readonly record struct V759_LastLayer(int Sequence);
    public static BlockDigPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockDigPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var status = reader.ReadVarInt();
            var location = reader.ReadType<Position>(protocolVersion);
            var face = reader.ReadSignedByte();
            return new BlockDigPacket(status, location, face);
        }

        if (protocolVersion >= 759)
        {
            var status = reader.ReadVarInt();
            var location = reader.ReadType<Position>(protocolVersion);
            var face = reader.ReadSignedByte();
            var sequence = reader.ReadVarInt();
            return new BlockDigPacket(status, location, face, V759_Last: new V759_LastLayer(sequence));
        }

        throw new System.NotSupportedException($"BlockDigPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockDigPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            writer.WriteVarInt(Status);
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteSignedByte((sbyte)Face);
            return;
        }

        if (protocolVersion >= 759)
        {
            var layer = V759_Last ?? throw new WrongLayerException("BlockDigPacket", protocolVersion, "V759_Last");
            int Sequence = layer.Sequence;
            writer.WriteVarInt(Status);
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteSignedByte((sbyte)Face);
            writer.WriteVarInt(Sequence);
            return;
        }

        throw new System.NotSupportedException($"BlockDigPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Status");
        writer.WriteNumberValue(Status);
        writer.WritePropertyName("Location");
        Location.WriteJson(writer);
        writer.WritePropertyName("Face");
        writer.WriteNumberValue(Face);
        if (V759_Last is { } v759_Last)
        {
            writer.WritePropertyName("Sequence");
            writer.WriteNumberValue(v759_Last.Sequence);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.block_dig", "BlockDig", PacketPhase.Play, PacketDirection.Serverbound, 4);

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
