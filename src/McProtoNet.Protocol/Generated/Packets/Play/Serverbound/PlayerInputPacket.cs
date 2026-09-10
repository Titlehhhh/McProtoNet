using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.player_input", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Inputs", "PlayerInputFlags")]
public sealed partial record PlayerInputPacket(PlayerInputFlags Inputs) : IPacket<PlayerInputPacket>, IPacket
{
    public static PlayerInputPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerInputPacket>(protocolVersion);
        var inputs = reader.ReadType<PlayerInputFlags>(protocolVersion);
        return new PlayerInputPacket(inputs);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerInputPacket>(protocolVersion);
        writer.WriteType<PlayerInputFlags>(Inputs, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toServer.player_input", "PlayerInput", PacketPhase.Play, PacketDirection.Serverbound, 37);

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
