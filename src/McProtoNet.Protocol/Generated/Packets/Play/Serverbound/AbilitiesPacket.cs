using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.abilities", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Flags", "int")]
public sealed partial record AbilitiesPacket(int Flags) : IPacket<AbilitiesPacket>, IPacket
{
    public static AbilitiesPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AbilitiesPacket>(protocolVersion);
        var flags = reader.ReadSignedByte();
        return new AbilitiesPacket(flags);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AbilitiesPacket>(protocolVersion);
        writer.WriteSignedByte((sbyte)Flags);
    }

    public static PacketIdentity Identity => new("play.toServer.abilities", "Abilities", PacketPhase.Play, PacketDirection.Serverbound, 0);

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
