namespace McProtoNet.Protocol340;

public sealed class LoginRejectedException : Exception
{
    public LoginRejectedException(string reason) : base(reason)
    {

    }
}
