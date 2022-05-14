namespace McProtoNet.Networking
{
    public class PacketSentEventArgs : EventArgs
    {
        public Packet Packet { get; private set; }
        public PacketSentEventArgs(Packet packet)
        {
            Packet = packet;
        }
    }
}
