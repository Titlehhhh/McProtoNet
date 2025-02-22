using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("WindowClick", PacketState.Play, PacketDirection.Serverbound)]
public partial class WindowClickPacket : IClientPacket
{
    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        throw new NotImplementedException();
    }

    [PacketSubInfo(340, 754)]
    public partial class V340_754 : WindowClickPacket
    {
        public byte WindowId { get; set; }
        public short Slot { get; set; }
        public sbyte MouseButton { get; set; }
        public short Action { get; set; }
        public sbyte Mode { get; set; }
        public Slot? Item { get; set; }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            byte windowId, short slot, sbyte mouseButton, short action, sbyte mode, Slot? item)
        {
            writer.WriteUnsignedByte(windowId);
            writer.WriteSignedShort(slot);
            writer.WriteSignedByte(mouseButton);
            writer.WriteSignedShort(action);
            writer.WriteSignedByte(mode);
            writer.WriteSlot(item, protocolVersion);
        }

        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, WindowId, Slot, MouseButton, Action, Mode, Item);
        }
    }
}