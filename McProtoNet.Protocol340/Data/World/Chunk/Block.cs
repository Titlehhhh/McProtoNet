namespace McProtoNet.Protocol340.Data.World.Chunk
{
    public record struct  Block
    {
      
        public readonly ushort Id;
        public readonly byte Data;

        public Block(uint id)
        {
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
