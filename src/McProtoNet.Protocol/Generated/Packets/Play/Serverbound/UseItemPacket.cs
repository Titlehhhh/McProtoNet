using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.use_item", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Hand", "int")]
[PacketField("Sequence", "int", Group = "V759_766", From = 759, To = 766)]
[PacketField("Sequence", "int", Group = "V767_Last", From = 767)]
[PacketField("Rotation", "Vec2f", Group = "V767_Last", From = 767)]
public sealed partial record UseItemPacket(int Hand, UseItemPacket.V759_766Layer? V759_766 = null, UseItemPacket.V767_LastLayer? V767_Last = null) : IPacket<UseItemPacket>, IPacket
{
    public readonly record struct V759_766Layer(int Sequence);
    public readonly record struct V767_LastLayer(int Sequence, Vec2f Rotation);
    public static UseItemPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UseItemPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var hand = reader.ReadVarInt();
            return new UseItemPacket(hand);
        }

        if (protocolVersion >= 759 && protocolVersion <= 766)
        {
            var hand = reader.ReadVarInt();
            var sequence = reader.ReadVarInt();
            return new UseItemPacket(hand, V759_766: new V759_766Layer(sequence));
        }

        if (protocolVersion >= 767)
        {
            var hand = reader.ReadVarInt();
            var sequence = reader.ReadVarInt();
            var rotation = reader.ReadType<Vec2f>(protocolVersion);
            return new UseItemPacket(hand, V767_Last: new V767_LastLayer(sequence, rotation));
        }

        throw new System.NotSupportedException($"UseItemPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UseItemPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            writer.WriteVarInt(Hand);
            return;
        }

        if (protocolVersion >= 759 && protocolVersion <= 766)
        {
            var layer = V759_766 ?? throw new WrongLayerException("UseItemPacket", protocolVersion, "V759_766");
            int Sequence = layer.Sequence;
            writer.WriteVarInt(Hand);
            writer.WriteVarInt(Sequence);
            return;
        }

        if (protocolVersion >= 767)
        {
            var layer = V767_Last ?? throw new WrongLayerException("UseItemPacket", protocolVersion, "V767_Last");
            int Sequence = layer.Sequence;
            Vec2f Rotation = layer.Rotation;
            writer.WriteVarInt(Hand);
            writer.WriteVarInt(Sequence);
            writer.WriteType<Vec2f>(Rotation, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"UseItemPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Hand");
        writer.WriteNumberValue(Hand);
        if (V759_766 is { } v759_766)
        {
            writer.WritePropertyName("Sequence");
            writer.WriteNumberValue(v759_766.Sequence);
        }
        else if (V767_Last is { } v767_Last)
        {
            writer.WritePropertyName("Sequence");
            writer.WriteNumberValue(v767_Last.Sequence);
            writer.WritePropertyName("Rotation");
            v767_Last.Rotation.WriteJson(writer);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.use_item", "UseItem", PacketPhase.Play, PacketDirection.Serverbound, 67);

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
