using McProtoNet.Core.IO;

namespace McProtoNet.HighPerfomance
{
    public struct Packet
    {
        public readonly int Id;
        public readonly MemoryStream Data;

        public Packet(int id, MemoryStream data)
        {
            Id = id;
            Data = data;
        }
    }
}
