using ProtoLib.API.Networking;
using ProtoLib.API.Types.Chat;
using ProtoLib.Geometry;

namespace ProtoLib.API
{
    public interface IMinecraftHandler
    {
        void OnDisconnect(Exception e);
        void OnDisconnect(string reason);

        void OnPacketReceived(IPacket packet);

        void OnLoginSucces(Guid uuid);
        void OnLoginReject(ChatMessage message);

        void OnGameJoined();

        void OnChat(string message);
        void OnPositionRotation(Point3 pos, Rotation rot, bool onGround);
    }
}
