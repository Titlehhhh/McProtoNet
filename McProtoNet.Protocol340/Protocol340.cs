using McProtoNet.Core.Packets;


namespace McProtoNet.Protocol340
{
    public class Protocol340 : IProtocol
    {

        public Protocol340()
        {

        }


        public int NumberVersion => 340;

        public PacketSide Side { get; set; }

        public IPacketCollection PacketCollection { get; private set; } = new PacketCollection340();

        public void Dispose()
        {
            this.PacketCollection.Dispose();
        }

    }
}