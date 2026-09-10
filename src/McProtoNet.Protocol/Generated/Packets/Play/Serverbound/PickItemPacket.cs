using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, 768)]
[Packet("play.toServer.pick_item", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Slot", "int")]
public sealed partial record PickItemPacket(int Slot) : IPacket<PickItemPacket>, IPacket
{
    public static PickItemPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PickItemPacket>(protocolVersion);
        var slot = reader.ReadVarInt();
        return new PickItemPacket(slot);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PickItemPacket>(protocolVersion);
        writer.WriteVarInt(Slot);
    }

    public static PacketIdentity Identity => new("play.toServer.pick_item", "PickItem", PacketPhase.Play, PacketDirection.Serverbound, 33);

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
