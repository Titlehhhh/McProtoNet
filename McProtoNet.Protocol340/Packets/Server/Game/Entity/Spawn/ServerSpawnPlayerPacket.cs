namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerSpawnPlayerPacket : Packet<Protocol340>
    {
        public int EntityId { get; private set; }
        public Guid UUID { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        //this.entityId = in.readVarInt();
        //this.uuid = in.readUUID();
        //this.x = in.readDouble();
        //this.y = in.readDouble();
        //this.z = in.readDouble();
        //this.yaw = in.readByte() * 360 / 256f;
        //this.pitch = in.readByte() * 360 / 256f;
        //this.metadata = NetUtil.readEntityMetadata(in);
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            EntityId = stream.ReadVarInt();
            UUID = stream.ReadUUID();
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            Yaw = stream.ReadSignedByte() * 360 / 256f;
            Pitch = stream.ReadSignedByte() * 360 / 256f;


        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerSpawnPlayerPacket() { }
    }

}
