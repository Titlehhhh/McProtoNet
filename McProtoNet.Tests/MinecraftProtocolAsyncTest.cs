using McProtoNet.Core.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace McProtoNet.Tests
{
    [TestClass]
    public class MinecraftProtocolAsyncTest
    {
        [TestMethod("Тест MinecraftProtocol со сжатием (короткий пакет)")]
        public void TestReadWriteWithCompressionShort()
        {
            Task.Run(async () =>
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
                        await protocol.SendPacketAsync(data, id);
                    }
                    ms.Position = 0;
                    (int afterId, MemoryStream readStream) = await protocol.ReadNextPacketAsync();
                    byte[] after = readStream.ToArray();

                    CollectionAssert.AreEqual(before, after, "Пакеты не совпадают");
                }
            }).GetAwaiter().GetResult();
        }
        [TestMethod("Тест MinecraftProtocol со сжатием (длинный пакет)")]
        public void TestReadWriteWithCompressionLong()
        {
            Task.Run(async () =>
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
                        await protocol.SendPacketAsync(data, id);
                    }
                    ms.Position = 0;
                    (int afterId, MemoryStream readStream) = await protocol.ReadNextPacketAsync();
                    byte[] after = readStream.ToArray();

                    CollectionAssert.AreEqual(before, after, "Пакеты не совпадают");
                }
            }).GetAwaiter().GetResult();
        }


        [TestMethod("Тест MinecraftProtocol без сжатия")]
        public void TestReadWriteWithoutCompression()
        {
            Task.Run(async () =>
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
                        await protocol.SendPacketAsync(data, id);
                    }
                    ms.Position = 0;
                    (int afterId, MemoryStream readStream) = await protocol.ReadNextPacketAsync();
                    byte[] after = readStream.ToArray();

                    CollectionAssert.AreEqual(before, after, "Пакеты не совпадают");
                }
            }).GetAwaiter().GetResult();
        }

    }
}