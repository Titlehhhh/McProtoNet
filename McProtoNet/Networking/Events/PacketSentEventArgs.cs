namespace McProtoNet.Networking
{
    public class PacketSentEventArgs : EventArgs
    {
        public IPacket Packet { get; private set; }
        public PacketSentEventArgs(IPacket packet)
        {
            Packet = packet;
        }
    }
}
