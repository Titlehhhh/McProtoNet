using McProtoNet.API;

namespace McProtoNet.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> AuthAsync(AuthInfo authInfo, out GameProfile gameProfile);
    }
}