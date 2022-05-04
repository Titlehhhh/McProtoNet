using ProtoLib.API.Networking;

namespace ProtoLib.API.Protocol
{
    public sealed class DefaultPacketProviderServer : IPacketProviderServer
    {
        public Dictionary<int, Type> StatusPackets { get; private set; }

        public Dictionary<int, Type> LoginPackets { get; private set; }

        public Dictionary<int, Type> GamePackets { get; private set; }

        public DefaultPacketProviderServer(Dictionary<int, Type> statusPackets, Dictionary<int, Type> loginPackets, Dictionary<int, Type> gamePackets)
        {
            if (statusPackets is null)
                throw new ArgumentNullException(nameof(statusPackets));
            if (loginPackets is null)
                throw new ArgumentNullException(nameof(loginPackets));
            if (gamePackets is null)
                throw new ArgumentNullException(nameof(gamePackets));
            CheckAssignable(statusPackets, nameof(statusPackets));
            CheckAssignable(loginPackets, nameof(loginPackets));
            CheckAssignable(gamePackets, nameof(gamePackets));


            StatusPackets = statusPackets;
            LoginPackets = loginPackets;
            GamePackets = gamePackets;

        }
        public DefaultPacketProviderServer()
        {
            StatusPackets = new Dictionary<int, Type>();
            LoginPackets = new Dictionary<int, Type>();
            GamePackets = new Dictionary<int, Type>();
        }
        private static void CheckAssignable(Dictionary<int, Type> collection, string name)
        {
            bool result = collection.All(item => item.Value.IsAssignableTo(typeof(IPacket)));
            if (!result)
            {
                throw new InvalidOperationException("В словаре содержится тип не реализующий IPacket. Имя параметра: " + name);
            }
        }
    }
}
