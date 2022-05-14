namespace McProtoNet.Networking
{
    public class PacketSendEventArgs : EventArgs
    {
        public Packet Packet { get; private set; }
        public PacketSendEventArgs(Packet packet)
        {
            Packet = packet;
        }
    }
}
