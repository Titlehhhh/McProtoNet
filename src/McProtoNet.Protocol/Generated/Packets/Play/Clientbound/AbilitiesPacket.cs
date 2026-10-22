using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.abilities", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Flags", "int")]
[PacketField("FlyingSpeed", "float")]
[PacketField("WalkingSpeed", "float")]
public sealed partial record AbilitiesPacket(int Flags, float FlyingSpeed, float WalkingSpeed) : IPacket<AbilitiesPacket>, IPacket
{
    public static AbilitiesPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AbilitiesPacket>(protocolVersion);
        var flags = reader.ReadSignedByte();
        var flyingSpeed = reader.ReadFloat();
        var walkingSpeed = reader.ReadFloat();
        return new AbilitiesPacket(flags, flyingSpeed, walkingSpeed);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AbilitiesPacket>(protocolVersion);
        writer.WriteSignedByte((sbyte)Flags);
        writer.WriteFloat(FlyingSpeed);
        writer.WriteFloat(WalkingSpeed);
    }

    public static PacketIdentity Identity => new("play.toClient.abilities", "Abilities", PacketPhase.Play, PacketDirection.Clientbound, 0);

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
