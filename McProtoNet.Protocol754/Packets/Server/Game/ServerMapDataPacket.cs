using McProtoNet.Protocol754.Data;

namespace McProtoNet.Protocol754.Packets.Server
{

    
    public sealed class ServerMapDataPacket : Packet<Protocol754>
    {

        public int MapId { get; set; }
        public byte Scale { get; set; }
        public bool TrackingPosition { get; set; }
        public bool Locked { get; set; }
        public MapIcon[] Icons { get; private set; }
        public MapData Data { get; set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            this.MapId = stream.ReadVarInt();
            this.Scale = stream.ReadUnsignedByte();
            this.TrackingPosition = stream.ReadBoolean();
            this.Locked = stream.ReadBoolean();
            this.Icons = new MapIcon[stream.ReadVarInt()];
            for (int index = 0; index < this.Icons.Length; index++)
            {
                var type = (MapIconType)stream.ReadVarInt();
                int x = stream.ReadUnsignedByte();
                int z = stream.ReadUnsignedByte();
                int rotation = stream.ReadUnsignedByte();
                string displayName = "";
                if (stream.ReadBoolean())
                {
                    displayName = stream.ReadString();
                }

                this.Icons[index] = new MapIcon(type, x, z, rotation, displayName);
            }

            int columns = stream.ReadUnsignedByte();
            if (columns > 0)
            {
                int rows = stream.ReadUnsignedByte();
                int x = stream.ReadUnsignedByte();
                int y = stream.ReadUnsignedByte();
                byte[] data = stream.ReadByteArray();

                this.Data = new MapData(columns, rows, x, y, data);
            }
        }
        public ServerMapDataPacket() { }
    }
}

