using McProtoNet.Core;
using System.Net;

namespace McProtoNet
{
    public class Client<TProto> : IClient<TProto> where TProto : IProtocol, new()
    {
        public EndPoint Address { get; private set; }

        public PacketListener<TProto> Listener { get; private set; }

        public Client(EndPoint address, PacketListener<TProto> listener)
        {
            Address = address;
            Listener = listener;
        }
    }
}
