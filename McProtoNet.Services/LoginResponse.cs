namespace McProtoNet.Services
{
    public class LoginResponse
    {
        public string UUID { get; private set; }
        public LoginResult Result { get; private set; }
        public LoginResponse(string uuid, LoginResult result)
        {
            UUID = uuid;
            Result = result;
        }
    }
}