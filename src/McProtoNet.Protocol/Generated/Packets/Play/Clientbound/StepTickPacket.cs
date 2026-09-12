using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.step_tick", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("TickSteps", "int")]
public sealed partial record StepTickPacket(int TickSteps) : IPacket<StepTickPacket>, IPacket
{
    public static StepTickPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StepTickPacket>(protocolVersion);
        var tickSteps = reader.ReadVarInt();
        return new StepTickPacket(tickSteps);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StepTickPacket>(protocolVersion);
        writer.WriteVarInt(TickSteps);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("TickSteps");
        writer.WriteNumberValue(TickSteps);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.step_tick", "StepTick", PacketPhase.Play, PacketDirection.Clientbound, 107);

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
