using static McProtoNet.Protocol340.Constans;
using McProtoNet.Protocol340.Data;

namespace McProtoNet.Protocol340.Packets.Client.Game
{


    public sealed class ClientPlayerActionPacket : Packet
    {
        public PlayerAction Action { get; set; }
        public Point3_Int Position { get; set; }
        public BlockFace Face { get; set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt((int)Action);
            long x = Position.X & POSITION_WRITE_SHIFT;
            long y = Position.Y & POSITION_X_SIZE;
            long z = Position.Z & POSITION_WRITE_SHIFT;

            stream.WriteLong(x << POSITION_X_SIZE | y << POSITION_Y_SIZE | z);

            stream.WriteByte((sbyte)Face);
        }

        public ClientPlayerActionPacket(PlayerAction action, Point3_Int position, BlockFace face)
        {
            Action = action;
            Position = position;
            Face = face;
        }

        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        
    }
}
