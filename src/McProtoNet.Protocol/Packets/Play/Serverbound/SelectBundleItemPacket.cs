using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("SelectBundleItem", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[PacketId(768, MinecraftVersion.LatestProtocol, 0x02)]
public sealed partial class SelectBundleItemPacket : IClientPacket
{
    public int SlotId { get; set; }
    public int SelectedItemIndex { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(SlotId);
    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => SlotId = reader.ReadVarInt();
    
    // Since there is only one range (768-last), we handle the second field directly after the first.
    // Note: The structure template requires a single Serialize/Deserialize method body if no switch is used, 
    // but since I am implementing it manually for the single case, I must ensure all fields are read/written sequentially.

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            writer.WriteVarInt(SlotId);
            writer.WriteVarInt(SelectedItemIndex);
        }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            SlotId = reader.ReadVarInt();
            SelectedItemIndex = reader.ReadVarInt();
        }
}