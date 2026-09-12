using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.use_entity", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Target", "int")]
[PacketField("Sneaking", "bool")]
[PacketField("Action", "InteractAction", Group = "VUntil774", To = 774)]
[PacketField("Hand", "int", Group = "V775_Last", From = 775)]
[PacketField("Location", "LpVec3", Group = "V775_Last", From = 775)]
public sealed partial record UseEntityPacket(int Target, bool Sneaking, UseEntityPacket.VUntil774Layer? VUntil774 = null, UseEntityPacket.V775_LastLayer? V775_Last = null) : IPacket<UseEntityPacket>, IPacket
{
    public readonly record struct VUntil774Layer(InteractAction Action);
    public readonly record struct V775_LastLayer(int Hand, LpVec3 Location);
    public static UseEntityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UseEntityPacket>(protocolVersion);
        if (protocolVersion <= 774)
        {
            var target = reader.ReadVarInt();
            var _mouse = reader.ReadVarInt();
            var action = InteractAction.Read(ref reader, protocolVersion, (int)_mouse);
            var sneaking = reader.ReadBoolean();
            return new UseEntityPacket(target, sneaking, VUntil774: new VUntil774Layer(action));
        }

        if (protocolVersion >= 775)
        {
            var target = reader.ReadVarInt();
            var hand = reader.ReadVarInt();
            var location = reader.ReadType<LpVec3>(protocolVersion);
            var sneaking = reader.ReadBoolean();
            return new UseEntityPacket(target, sneaking, V775_Last: new V775_LastLayer(hand, location));
        }

        throw new System.NotSupportedException($"UseEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UseEntityPacket>(protocolVersion);
        if (protocolVersion <= 774)
        {
            var layer = VUntil774 ?? throw new WrongLayerException("UseEntityPacket", protocolVersion, "VUntil774");
            InteractAction Action = layer.Action;
            writer.WriteVarInt(Target);
            writer.WriteVarInt(Action.Discriminator(protocolVersion));
            Action.Write(writer, protocolVersion);
            writer.WriteBoolean(Sneaking);
            return;
        }

        if (protocolVersion >= 775)
        {
            var layer = V775_Last ?? throw new WrongLayerException("UseEntityPacket", protocolVersion, "V775_Last");
            int Hand = layer.Hand;
            LpVec3 Location = layer.Location;
            writer.WriteVarInt(Target);
            writer.WriteVarInt(Hand);
            writer.WriteType<LpVec3>(Location, protocolVersion);
            writer.WriteBoolean(Sneaking);
            return;
        }

        throw new System.NotSupportedException($"UseEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Target");
        writer.WriteNumberValue(Target);
        writer.WritePropertyName("Sneaking");
        writer.WriteBooleanValue(Sneaking);
        if (VUntil774 is { } vUntil774)
        {
            writer.WritePropertyName("Action");
            vUntil774.Action.WriteJson(writer);
        }
        else if (V775_Last is { } v775_Last)
        {
            writer.WritePropertyName("Hand");
            writer.WriteNumberValue(v775_Last.Hand);
            writer.WritePropertyName("Location");
            v775_Last.Location.WriteJson(writer);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.use_entity", "UseEntity", PacketPhase.Play, PacketDirection.Serverbound, 66);

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
