using McProtoNet.Core;

namespace McProtoNet
{
    public interface IServer<TProto> where TProto : IProtocol, new()
    {
        event ClientConnectedHandler<TProto> ClientConnected;
        event ServerOnErrorHandler<TProto> OnError;
        event ServerStartedHandler<TProto> ServerStarted;
        event ServerStopedHandler<TProto> ServerStoped;

        void Dispose();
        void Start();
        void Stop();
    }
}