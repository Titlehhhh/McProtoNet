namespace McProtoNet
{
    public interface IClientObserver
    {
        void OnConnected();
        void OnLoginSucces(Guid uuid);
        void OnLoginFailed(string reason);
        void OnDisconnect(string reason);
        void OnDisconnect();
        void OnLoginDisconnect(string reason);
        void OnError(Exception e);

        void OnHandShake();
        void OnLoginStart();
    }
}
