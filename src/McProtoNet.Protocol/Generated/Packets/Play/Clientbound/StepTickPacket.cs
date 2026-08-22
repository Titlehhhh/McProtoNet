using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toClient.step_tick", "StepTick", PacketPhase.Play, PacketDirection.Clientbound, 98);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x6F;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x72;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 772)
        {
            id = 0x79;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x7E;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x80;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
