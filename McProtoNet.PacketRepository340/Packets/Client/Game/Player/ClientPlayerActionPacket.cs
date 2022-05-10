using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.Geometry;
using static McProtoNet.PacketRepository340.Constans;

namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientPlayerActionPacket : IPacket
    {
        public PlayerAction Action { get; set; }
        public Point3_Int Position { get; set; }
        public GeoBlockFace Face { get; set; }
        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt((int)Action);
            long x = Position.X & POSITION_WRITE_SHIFT;
            long y = Position.Y & POSITION_X_SIZE;
            long z = Position.Z & POSITION_WRITE_SHIFT;

            stream.WriteLong(x << POSITION_X_SIZE | y << POSITION_Y_SIZE | z);

            stream.WriteByte((sbyte)Face);
        }

        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientPlayerActionPacket(PlayerAction action, Point3_Int position, GeoBlockFace face)
        {
            Action = action;
            Position = position;
            Face = face;
        }
    }
}
