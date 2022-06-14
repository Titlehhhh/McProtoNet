namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerPlayerPositionRotationPacket : Packet
    {
        //this.x = in.readDouble();
        //this.y = in.readDouble();
        //this.z = in.readDouble();
        //this.yaw = in.readFloat();
        //this.pitch = in.readFloat();
        //this.relative = new ArrayList<PositionElement>();
        //int flags = in.readUnsignedByte();
        //for(PositionElement element : PositionElement.values()) {
        //int bit = 1 << MagicValues.value(Integer.class, element);
        //if((flags & bit) == bit) {
        //this.relative.add(element);
        //}
        //}
        //
        //this.teleportId = in.readVarInt();

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        public IList<PositionElement> Relative { get; private set; }

        public int TeleportId { get; private set; }

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();

            Yaw = stream.ReadFloat();
            Pitch = stream.ReadFloat();

            byte flags = stream.ReadUnsignedByte();
            List<PositionElement> relative = new();
            foreach (PositionElement pos in Enum.GetValues(typeof(PositionElement)))
            {
                var bit = 1 << (int)pos;
                if ((flags & bit) == bit)
                {
                    relative.Add(pos);
                }
            }
            Relative = relative;

            TeleportId = stream.ReadVarInt();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPlayerPositionRotationPacket() { }
    }

}
