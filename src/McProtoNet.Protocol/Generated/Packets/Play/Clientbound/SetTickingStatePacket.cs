using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.set_ticking_state", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("TickRate", "float")]
[PacketField("IsFrozen", "bool")]
public sealed partial record SetTickingStatePacket(float TickRate, bool IsFrozen) : IPacket<SetTickingStatePacket>, IPacket
{
    public static SetTickingStatePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTickingStatePacket>(protocolVersion);
        var tickRate = reader.ReadFloat();
        var isFrozen = reader.ReadBoolean();
        return new SetTickingStatePacket(tickRate, isFrozen);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTickingStatePacket>(protocolVersion);
        writer.WriteFloat(TickRate);
        writer.WriteBoolean(IsFrozen);
    }

    public static PacketIdentity Identity => new("play.toClient.set_ticking_state", "SetTickingState", PacketPhase.Play, PacketDirection.Clientbound, 92);

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
