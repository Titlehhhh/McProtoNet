namespace McProtoNet.Exceptions
{
    public class LoginRejectException : Exception
    {
        public override string Message { get; }

        public LoginRejectException(string message)
        {
            Message = message;
        }
    }

}
