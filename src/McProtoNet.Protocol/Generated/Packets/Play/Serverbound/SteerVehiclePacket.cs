using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, 767)]
[Packet("play.toServer.steer_vehicle", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Sideways", "float")]
[PacketField("Forward", "float")]
[PacketField("Jump", "int")]
public sealed partial record SteerVehiclePacket(float Sideways, float Forward, int Jump) : IPacket<SteerVehiclePacket>, IPacket
{
    public static SteerVehiclePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SteerVehiclePacket>(protocolVersion);
        var sideways = reader.ReadFloat();
        var forward = reader.ReadFloat();
        var jump = reader.ReadUnsignedByte();
        return new SteerVehiclePacket(sideways, forward, jump);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SteerVehiclePacket>(protocolVersion);
        writer.WriteFloat(Sideways);
        writer.WriteFloat(Forward);
        writer.WriteUnsignedByte((byte)Jump);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Sideways");
        if (double.IsFinite(Sideways))
            writer.WriteNumberValue(Sideways);
        else
            writer.WriteStringValue(double.IsNaN(Sideways) ? "NaN" : Sideways > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Forward");
        if (double.IsFinite(Forward))
            writer.WriteNumberValue(Forward);
        else
            writer.WriteStringValue(double.IsNaN(Forward) ? "NaN" : Forward > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Jump");
        writer.WriteNumberValue(Jump);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.steer_vehicle", "SteerVehicle", PacketPhase.Play, PacketDirection.Serverbound, 56);

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
