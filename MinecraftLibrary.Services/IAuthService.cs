using ProtoLib.API;

namespace ProtoLib.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> AuthAsync(AuthInfo authInfo, out GameProfile gameProfile);
    }
}