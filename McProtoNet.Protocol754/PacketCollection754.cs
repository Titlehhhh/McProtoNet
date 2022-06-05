using McProtoNet.Protocol754.Packets.Client;
using McProtoNet.Protocol754.Packets.Server;
using System.Reflection;

namespace McProtoNet.Protocol754
{

    public sealed class PacketCollection754 : IPacketCollection
    {
        public static readonly Dictionary<PacketCategory, Dictionary<int, Type>> ClientPackets = new();
        public static readonly Dictionary<PacketCategory, Dictionary<int, Type>> ServerPackets = new();
        static PacketCollection754()
        {
            Type tpacket = typeof(Packet);
            var list = new List<Type>();
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (tpacket.IsAssignableFrom(item))
                {
                    list.Add(item);
                }
            }


            Dictionary<int, Type> ClientGamePackets = new();
            Dictionary<int, Type> ServerGamePackets = new();

            foreach (Type item in list)
            {
                PacketInfoAttribute packetInfo = item.GetCustomAttribute<PacketInfoAttribute>();

                if (packetInfo.Category != PacketCategory.Game)
                    continue;
                if (packetInfo.Side == PacketSide.Client)
                {
                    ClientGamePackets.Add(packetInfo.ID, item);
                }
                else
                {
                    ServerGamePackets.Add(packetInfo.ID, item);
                }
            }


            ClientPackets.Add(PacketCategory.Game, ClientGamePackets);
            ServerPackets.Add(PacketCategory.Game, ServerGamePackets);

            ServerPackets.Add(PacketCategory.Login, new Dictionary<int, Type>()
                {
                    {0x00, typeof(LoginDisconnectPacket) },
                    {0x01,typeof(EncryptionRequestPacket) },
                    {0x02,typeof(LoginSuccessPacket) },
                    {0x03,typeof(LoginSetCompressionPacket) },
                    {0x04,typeof(LoginPluginRequestPacket) }
                });
            ClientPackets.Add(PacketCategory.Login, new Dictionary<int, Type>()
                {
                    {0x00, typeof(LoginStartPacket) },
                    {0x01, typeof(EncryptionResponsePacket) },
                    {0x02,typeof(LoginPluginResponsePacket) }
                });

            ClientPackets.Add(PacketCategory.HandShake, new Dictionary<int, Type>()
            {
                {0x00, typeof(HandShakePacket) }
            });



        }

        public int TargetProtocolVersion => 754;

        public Dictionary<int, Type> GetClientPacketsByCategory(PacketCategory category)
        {
            return ClientPackets[category];
        }

        public Dictionary<int, Type> GetServerPacketsByCategory(PacketCategory category)
        {
            return ServerPackets[category];
        }

        public Dictionary<PacketCategory, IPacketProvider> GetAllPackets( PacketSide side)
        {
           

            var categories = new List<PacketCategory>
                {
                    PacketCategory.HandShake,
                    PacketCategory.Status,
                    PacketCategory.Login,
                    PacketCategory.Game
                };

            if (side == PacketSide.Client)
            {
                
                var all = categories
                    .ToDictionary(k => k, v => (IPacketProvider)new PacketProvider(ClientPackets[v], ServerPackets[v]));

                return all;
            }
            else 
            {

                var all = categories
                    .ToDictionary(k => k, v => (IPacketProvider)new PacketProvider(ServerPackets[v], ClientPackets[v]));

                return all;
            }
        }
    }
}
