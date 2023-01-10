using McProtoNet.Core;
using System.Net;

namespace McProtoNet
{
    public interface IClient<TProto> where TProto : IProtocol, new()
    {
        public EndPoint Address { get; }
        public PacketListener<TProto> Listener { get; }
    }
}
