using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;
using McProtoNet.Geometry;

namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x29, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientUpdateJigsawBlockPacket : IPacket
    {
        public Point3_Int Position { get; private set; }
        public string Name { get; private set; }
        public string Traget { get; private set; }
        public string Pool { get; private set; }
        public string FinalState { get; private set; }
        public string JointType { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            //stream.WriteVarInt(Position);
            stream.WriteString(Name);
            stream.WriteString(Traget);
            stream.WriteString(Pool);
            stream.WriteString(FinalState);
            stream.WriteString(JointType);
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            //Position = (Point3_Int)stream.ReadVarInt();
            Name = stream.ReadString();
            Traget = stream.ReadString();
            Pool = stream.ReadString();
            FinalState = stream.ReadString();
            JointType = stream.ReadString();
        }
        public ClientUpdateJigsawBlockPacket() { }

        public ClientUpdateJigsawBlockPacket(Point3_Int Position, string Name, string Traget, string Pool, string FinalState, string JointType)
        {
            this.Position = Position;
            this.Name = Name;
            this.Traget = Traget;
            this.Pool = Pool;
            this.FinalState = FinalState;
            this.JointType = JointType;
        }
    }
}
