using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.change_gamemode", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Mode", "int")]
public sealed partial record ChangeGamemodePacket(int Mode) : IPacket<ChangeGamemodePacket>, IPacket
{
    public static ChangeGamemodePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChangeGamemodePacket>(protocolVersion);
        var mode = reader.ReadVarInt();
        return new ChangeGamemodePacket(mode);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChangeGamemodePacket>(protocolVersion);
        writer.WriteVarInt(Mode);
    }

    public static PacketIdentity Identity => new("play.toServer.change_gamemode", "ChangeGamemode", PacketPhase.Play, PacketDirection.Serverbound, 4);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 771 && protocolVersion <= 772)
        {
            id = 0x04;
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
