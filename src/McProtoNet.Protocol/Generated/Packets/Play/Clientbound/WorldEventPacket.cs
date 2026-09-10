using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.world_event", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EffectId", "int")]
[PacketField("Location", "Position")]
[PacketField("Data", "int")]
[PacketField("Global", "bool")]
public sealed partial record WorldEventPacket(int EffectId, Position Location, int Data, bool Global) : IPacket<WorldEventPacket>, IPacket
{
    public static WorldEventPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldEventPacket>(protocolVersion);
        var effectId = reader.ReadSignedInt();
        var location = reader.ReadType<Position>(protocolVersion);
        var data = reader.ReadSignedInt();
        var global = reader.ReadBoolean();
        return new WorldEventPacket(effectId, location, data, global);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldEventPacket>(protocolVersion);
        writer.WriteSignedInt(EffectId);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteSignedInt(Data);
        writer.WriteBoolean(Global);
    }

    public static PacketIdentity Identity => new("play.toClient.world_event", "WorldEvent", PacketPhase.Play, PacketDirection.Clientbound, 133);

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
