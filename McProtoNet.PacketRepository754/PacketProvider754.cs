﻿using System.Reflection;

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

                CLIENTPACKETS = new DefaultPacketProviderClient();
                SERVERPACKETS = new DefaultPacketProviderServer();

                foreach (var item in list)
                {
                    PacketInfoAttribute packetInfo = item.GetCustomAttribute<PacketInfoAttribute>();
                    if (packetInfo.Side == McProtoNet.PacketSide.Client)
                    {


                        switch (packetInfo.Category)
                        {
                            case McProtoNet.PacketCategory.Status:
                                CLIENTPACKETS.StatusPackets.Add(packetInfo.ID, item);
                                break;
                            case McProtoNet.PacketCategory.HandShake:
                                CLIENTPACKETS.HandShakePackets.Add(packetInfo.ID, item);
                                break;
                            case McProtoNet.PacketCategory.Login:
                                CLIENTPACKETS.LoginPackets.Add(packetInfo.ID, item);
                                break;
                            case McProtoNet.PacketCategory.Game:
                                CLIENTPACKETS.GamePackets.Add(packetInfo.ID, item);
                                break;
                        }

                    }
                    else
                    {


                        switch (packetInfo.Category)
                        {
                            case McProtoNet.PacketCategory.Status:
                                SERVERPACKETS.StatusPackets.Add(packetInfo.ID, item);
                                break;
                            case McProtoNet.PacketCategory.Login:
                                SERVERPACKETS.LoginPackets.Add(packetInfo.ID, item);
                                break;
                            case McProtoNet.PacketCategory.Game:
                                SERVERPACKETS.GamePackets.Add(packetInfo.ID, item);
                                break;
                        }

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