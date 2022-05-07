using McProtoNet.API;

namespace McProtoNet.Utils
{
    public class AuthService : IAuthService
    {
        public Task<LoginResponse> AuthAsync(AuthInfo authInfo, out GameProfile gameProfile)
        {
            throw new NotImplementedException();
        }
    }
}