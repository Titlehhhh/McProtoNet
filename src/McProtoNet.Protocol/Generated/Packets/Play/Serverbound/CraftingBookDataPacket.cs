using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 736)]
[Packet("play.toServer.crafting_book_data", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Data", "CraftingBookDataAction")]
public sealed partial record CraftingBookDataPacket(CraftingBookDataAction Data) : IPacket<CraftingBookDataPacket>, IPacket
{
    public static CraftingBookDataPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CraftingBookDataPacket>(protocolVersion);
        var _type = reader.ReadVarInt();
        var data = CraftingBookDataAction.Read(ref reader, protocolVersion, (int)_type);
        return new CraftingBookDataPacket(data);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CraftingBookDataPacket>(protocolVersion);
        writer.WriteVarInt(Data.Discriminator(protocolVersion));
        Data.Write(writer, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toServer.crafting_book_data", "CraftingBookData", PacketPhase.Play, PacketDirection.Serverbound, 19);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x1E;
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
