namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x2B, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientUpdateSignPacket : Packet
    {
        public Point3_Int Position { get; private set; }
        public string[] Lines { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WritePoint3_int(Position);
            foreach (var line in Lines)
            {
                stream.WriteString(line);
            }
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientUpdateSignPacket(Point3_Int position, string[] lines)
        {
            Position = position;
            Lines = new string[lines.Length];

            Array.Copy(lines, Lines, lines.Length);
        }

        public ClientUpdateSignPacket() { }


    }
}
