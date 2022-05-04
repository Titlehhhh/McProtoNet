
namespace McProtoNet.API.Networking
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public int ID { get; private set; }
        public IPacket Packet { get; private set; }

        public PacketReceivedEventArgs(int iD, IPacket packet)
        {
            ID = iD;
            this.Packet = packet;
        }
    }
}
