using McProtoNet.Core.Packets;
using McProtoNet.Core.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace McProtoNet.Tests
{
    [TestClass]
    public class MinecraftProtocolSyncTest
    {
        [TestMethod("Тест MinecraftProtocol со сжатием (короткий пакет)")]
        public void TestReadWriteWithCompressionShort()
        {

            Random rand = new Random();

            using (MemoryStream ms = new MemoryStream())
            using (NetworkMinecraftStream netmc = new NetworkMinecraftStream(ms))
            using (IMinecraftProtocol protocol = new MinecraftProtocol(netmc, true))
            {
                protocol.SwitchCompression(256);

                byte[] before = new byte[100];
                rand.NextBytes(before);
                int id = rand.Next(0, 100);

                using (MemoryStream data = new MemoryStream(before))
                {
                    protocol.SendPacket(data, id);

                }
                ms.Position = 0;


                (int afterId, MemoryStream readStream) = protocol.ReadNextPacket();

                byte[] after = readStream.ToArray();

                CollectionAssert.AreEqual(before, after, "Пакеты не совпадают");


            }

        }
        [TestMethod("Тест MinecraftProtocol со сжатием (длинный пакет)")]
        public void TestReadWriteWithCompressionLong()
        {
            Random rand = new Random();

            using (MemoryStream ms = new MemoryStream())
            using (NetworkMinecraftStream netmc = new NetworkMinecraftStream(ms))
            using (IMinecraftProtocol protocol = new MinecraftProtocol(netmc, true))
            {
                protocol.SwitchCompression(256);

                byte[] before = new byte[1000];
                rand.NextBytes(before);
                int id = rand.Next(0, 100);

                using (MemoryStream data = new MemoryStream(before))
                {
                    protocol.SendPacket(data, id);
                }
                ms.Position = 0;
                (int afterId, MemoryStream readStream) = protocol.ReadNextPacket();
                byte[] after = readStream.ToArray();

                CollectionAssert.AreEqual(before, after, "Пакеты не совпадают");
            }

        }


        [TestMethod("Тест MinecraftProtocol без сжатия")]
        public void TestReadWriteWithoutCompression()
        {
            Random rand = new Random();

            using (MemoryStream ms = new MemoryStream())
            using (NetworkMinecraftStream netmc = new NetworkMinecraftStream(ms))
            using (IMinecraftProtocol protocol = new MinecraftProtocol(netmc, true))
            {
                byte[] before = new byte[1000];
                rand.NextBytes(before);
                int id = rand.Next(0, 100);

                using (MemoryStream data = new MemoryStream(before))
                {
                    protocol.SendPacket(data, id);
                }
                ms.Position = 0;
                (int afterId, MemoryStream readStream) = protocol.ReadNextPacket();
                byte[] after = readStream.ToArray();

                CollectionAssert.AreEqual(before, after, "Пакеты не совпадают");
            }

        }

    }
}