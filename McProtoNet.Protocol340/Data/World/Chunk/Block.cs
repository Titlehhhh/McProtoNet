namespace McProtoNet.Protocol340.Data.World.Chunk
{
    public record Block
    {
        public readonly uint Deb;
        public readonly ushort Id;
        public readonly byte Data;

        public Block(uint id)
        {
            Deb = id;
            //rawId >> 4, rawId & 0xF
            Id = (ushort)(id >> 4);
            Data = (byte)(id & 0xF);
        }

        public Block(ushort id, byte data)
        {
            Id = id;
            Data = data;
        }
    }
}
