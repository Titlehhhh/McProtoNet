using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.steer_boat", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("LeftPaddle", "bool")]
[PacketField("RightPaddle", "bool")]
public sealed partial record SteerBoatPacket(bool LeftPaddle, bool RightPaddle) : IPacket<SteerBoatPacket>, IPacket
{
    public static SteerBoatPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SteerBoatPacket>(protocolVersion);
        var leftPaddle = reader.ReadBoolean();
        var rightPaddle = reader.ReadBoolean();
        return new SteerBoatPacket(leftPaddle, rightPaddle);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SteerBoatPacket>(protocolVersion);
        writer.WriteBoolean(LeftPaddle);
        writer.WriteBoolean(RightPaddle);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("LeftPaddle");
        writer.WriteBooleanValue(LeftPaddle);
        writer.WritePropertyName("RightPaddle");
        writer.WriteBooleanValue(RightPaddle);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.steer_boat", "SteerBoat", PacketPhase.Play, PacketDirection.Serverbound, 55);

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
