using System.Reflection;

namespace McProtoNet.PacketRepository754
{
    public sealed class PacketProvider754 : IPacketProvider
    {
        private static readonly IPacketProviderClient CLIENTPACKETS;
        private static readonly IPacketProviderServer SERVERPACKETS;

        static PacketProvider754()
        {
            try
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

                CLIENTPACKETS = new PacketProviderClient();
                SERVERPACKETS = new PacketProviderServer();

                foreach (var item in list)
                {
                    PacketInfoAttribute packetInfo = item.GetCustomAttribute<PacketInfoAttribute>();
                    if (packetInfo.Side == PacketSide.Client)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            catch (Exception e)
            {
                using (StreamWriter sw = new StreamWriter("error.txt"))
                {
                    sw.WriteLine(e.StackTrace);
                }
            }
        }

        public IPacketProviderClient ClientPackets => CLIENTPACKETS;

        public IPacketProviderServer ServerPackets => SERVERPACKETS;

        public int TargetVersion => 754;
    }
}
