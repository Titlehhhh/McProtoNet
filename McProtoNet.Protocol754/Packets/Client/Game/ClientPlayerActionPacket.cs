using McProtoNet.Protocol754.Data;

namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x1B, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientPlayerActionPacket : Packet
    {
        public PlayerAction Action { get; set; }

        public Point3_Int Position { get; set; }

        public BlockFace Face { get; set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(Action);
            stream.WritePoint3_int(Position);
            stream.WriteUnsignedByte((byte)Face);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientPlayerActionPacket() { }

        public ClientPlayerActionPacket(PlayerAction action, Point3_Int position, BlockFace face)
        {
            Action = action;
            Position = position;
            Face = face;
        }
    }
}
