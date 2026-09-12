using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.arm_animation", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Hand", "int")]
public sealed partial record ArmAnimationPacket(int Hand) : IPacket<ArmAnimationPacket>, IPacket
{
    public static ArmAnimationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ArmAnimationPacket>(protocolVersion);
        var hand = reader.ReadVarInt();
        return new ArmAnimationPacket(hand);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ArmAnimationPacket>(protocolVersion);
        writer.WriteVarInt(Hand);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Hand");
        writer.WriteNumberValue(Hand);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.arm_animation", "ArmAnimation", PacketPhase.Play, PacketDirection.Serverbound, 2);

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
