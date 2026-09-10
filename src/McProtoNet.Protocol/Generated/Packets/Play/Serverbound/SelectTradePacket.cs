using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.select_trade", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Slot", "int")]
public sealed partial record SelectTradePacket(int Slot) : IPacket<SelectTradePacket>, IPacket
{
    public static SelectTradePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SelectTradePacket>(protocolVersion);
        var slot = reader.ReadVarInt();
        return new SelectTradePacket(slot);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SelectTradePacket>(protocolVersion);
        writer.WriteVarInt(Slot);
    }

    public static PacketIdentity Identity => new("play.toServer.select_trade", "SelectTrade", PacketPhase.Play, PacketDirection.Serverbound, 47);

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
