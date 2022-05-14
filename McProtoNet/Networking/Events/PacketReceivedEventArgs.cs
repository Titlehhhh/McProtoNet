
namespace McProtoNet.Networking
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public int ID { get; private set; }
        public Packet Packet { get; private set; }

        public PacketReceivedEventArgs(int iD, Packet packet)
        {
            ID = iD;
            this.Packet = packet;
        }
    }
}
