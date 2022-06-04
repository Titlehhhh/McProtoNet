namespace McProtoNet.Core
{
    public interface ISession
    {
        void Close();
        Task<bool> Login();
    }
}
