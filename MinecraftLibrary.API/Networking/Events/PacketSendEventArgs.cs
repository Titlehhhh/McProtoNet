namespace ProtoLib.API.Networking
{
    public class PacketSendEventArgs : EventArgs
    {
        public IPacket Packet { get; private set; }
        public PacketSendEventArgs(IPacket packet)
        {
            Packet = packet;
        }
    }
}
