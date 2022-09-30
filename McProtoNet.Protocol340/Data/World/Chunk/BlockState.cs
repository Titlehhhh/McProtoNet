using Org.BouncyCastle.Utilities;
using System;

namespace McProtoNet.Protocol340.Data.World.Chunk
{
    public class BlockState
    {
        public int Id { get; private set; }
        public int Data { get; private set; }

        public BlockState(int id, int data)
        {
            this.Id = id;
            this.Data = data;
        }
    }
}
