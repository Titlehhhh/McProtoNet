using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.close_window", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("WindowId", "int")]
public sealed partial record CloseWindowPacket(int WindowId) : IPacket<CloseWindowPacket>, IPacket
{
    public static CloseWindowPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CloseWindowPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var windowId = reader.ReadUnsignedByte();
            return new CloseWindowPacket(windowId);
        }

        if (protocolVersion >= 768)
        {
            var windowId = reader.ReadVarInt();
            return new CloseWindowPacket(windowId);
        }

        throw new System.NotSupportedException($"CloseWindowPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CloseWindowPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            writer.WriteUnsignedByte((byte)WindowId);
            return;
        }

        if (protocolVersion >= 768)
        {
            writer.WriteVarInt(WindowId);
            return;
        }

        throw new System.NotSupportedException($"CloseWindowPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.close_window", "CloseWindow", PacketPhase.Play, PacketDirection.Serverbound, 15);

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
