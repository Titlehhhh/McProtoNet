namespace McProtoNet.Protocol754
{
    public sealed class Protocol754 : IProtocol
    {
        public int NumberVersion => 754;

        public IPacketCollection PacketCollection { get; private set; } = new PacketCollection754();

        public void Dispose()
        {
            PacketCollection.Dispose();
        }
    }
}
