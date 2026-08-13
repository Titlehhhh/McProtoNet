using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toServer.player_input", "PlayerInput", PacketPhase.Play, PacketDirection.Serverbound, 32);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 772)
        {
            id = 0x2A;
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
