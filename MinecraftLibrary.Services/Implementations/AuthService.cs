using ProtoLib.API;

namespace ProtoLib.Services
{
    public class AuthService : IAuthService
    {
        public Task<LoginResponse> AuthAsync(AuthInfo authInfo, out GameProfile gameProfile)
        {
            throw new NotImplementedException();
        }
    }
}