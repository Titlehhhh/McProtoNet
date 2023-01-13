using McProtoNet.Core.IO;
using McProtoNet.Core.Packets;
using McProtoNet.Core.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace McProtoNet.Tests
{
    [TestClass]
    public class MinecraftProtocolTest
    {
        [TestMethod]
        public void TestReadWriteSync()
        {
            Dictionary<int, Type> packets = new Dictionary<int, Type>
            {
                {0x00, typeof(TestPacket) }
            };
            Random rand = new Random();

            using (MemoryStream ms = new MemoryStream())
            using (NetworkMinecraftStream netmc = new NetworkMinecraftStream(ms))
            using (IMinecraftProtocol protocol = new MinecraftProtocol(netmc, true))
            using (IPacketProvider packs = new PacketProvider(packets, packets))
            using (IPacketReaderWriter packetReaderWriter = new PacketReaderWriter(protocol, packs, true))
            {
                protocol.SwitchCompression(256);
                using var test = new MemoryStream();

                TestPacket excepted = new TestPacket(test);


                WriteArr("до:", test.ToArray());
                IMinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter(test);
                excepted.Write(writer);

                WriteArr("после:", test.ToArray());
                protocol.SendPacket(test, 0x00);
                WriteArr("FullData:", ms.ToArray());
                ms.Position = 0;

                TestPacket actual = (TestPacket)packetReaderWriter.ReadNextPacket();


                Assert.AreEqual(excepted, actual, "Пакеты не совпадают");
            }

        }
        public static void WriteArr(string mess, byte[] b)
        {
            Trace.WriteLine($"{mess} {{ {string.Join(", ", b)} }}");
        }

        [TestMethod]
        public void TestReadWriteAsync()
        {
            Task.Run(async () =>
            {
                Dictionary<int, Type> packets = new Dictionary<int, Type>
                {
                    {0x00, typeof(TestPacket) }
                };
                Random rand = new Random();
                for (int i = 0; i < 5; i++)
                {
                    using (MemoryStream ms = new MemoryStream())
                    using (NetworkMinecraftStream netmc = new NetworkMinecraftStream(ms))
                    using (IMinecraftProtocol protocol = new MinecraftProtocol(netmc, true))
                    using (IPacketProvider packs = new PacketProvider(packets, packets))
                    using (IPacketReaderWriter packetReaderWriter = new PacketReaderWriter(protocol, packs, true))
                    {
                        protocol.SwitchCompression(256);
                        TestPacket excepted = new TestPacket();

                        await packetReaderWriter.SendPacketAsync(excepted);
                        ms.Position = 0;

                        TestPacket actual = (TestPacket)await packetReaderWriter.ReadNextPacketAsync();

                        Assert.AreEqual(excepted, actual, "Пакеты не совпадают");
                    }
                }
            }).GetAwaiter().GetResult();
        }
    }
}