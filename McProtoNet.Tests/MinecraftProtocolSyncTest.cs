using McProtoNet.Core.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace McProtoNet.Tests
{
    [TestClass]
    public class MinecraftProtocolSyncTest
    {
        private static void WA(string m, byte[] a)
        {
            Debug.WriteLine($"{a}{string.Join(", ", a)}");
        }
        [TestMethod("Тест MinecraftProtocol со сжатием (короткий пакет)")]
        public void TestReadWriteWithCompressionShort()
        {

            Random rand = new Random();

            using (MemoryStream ms = new MemoryStream())
            using (MinecraftStream netmc = new MinecraftStream(ms))
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


                /* Необъединенное слияние из проекта "McProtoNet.Tests (net7.0)"
                До:
                                byte[] after = readStream.ToArray();

                                CollectionAssert.AreEqual(before, after, "Пакеты не совпадают");
                После:
                                byte[] after = readStream.ToArray();

                                CollectionAssert.AreEqual(before, after, "Пакеты не совпадают");
                */
                byte[] after = readStream.ToArray();

                CollectionAssert.AreEqual(before, after, "Пакеты не совпадают");


            }

        }
        [TestMethod("Тест MinecraftProtocol со сжатием (длинный пакет)")]
        public void TestReadWriteWithCompressionLong()
        {
            Random rand = new Random();

            using (MemoryStream ms = new MemoryStream())
            using (MinecraftStream netmc = new MinecraftStream(ms))
            using (IMinecraftProtocol protocol = new MinecraftProtocol(netmc, true))
            {

                protocol.SwitchCompression(256);

                byte[] before = new byte[1000];

                rand.NextBytes(before);
                WA("до: ", before);
                int id = rand.Next(0, 100);

                using (MemoryStream data = new MemoryStream(before))
                {
                    protocol.SendPacket(data, id);
                }
                ms.Position = 0;
                (int afterId, MemoryStream readStream) = protocol.ReadNextPacket();
                byte[] after = readStream.ToArray();
                WA("после: ", after);
                Assert.AreEqual(id, afterId, "Айди не совпадают");
                CollectionAssert.AreEqual(before, after, "Пакеты не совпадают");
            }

        }


        [TestMethod("Тест MinecraftProtocol без сжатия")]
        public void TestReadWriteWithoutCompression()
        {
            Random rand = new Random();

            using (MemoryStream ms = new MemoryStream())
            using (MinecraftStream netmc = new MinecraftStream(ms))
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