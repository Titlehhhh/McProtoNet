using McProtoNet.Core.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace McProtoNet.Tests
{
    [TestClass]
    public class TestVarInt
    {
        [TestMethod]
        public void TestReadWriteVarInt()
        {
            Random rand = new Random();
            for (int i = 0; i <= 1000; i++)
            {
                byte[] data = null;
                byte[] expected = new byte[1000];
                rand.NextBytes(expected);
                using (MemoryStream ms = new MemoryStream())
                {
                    IMinecraftPrimitiveWriter w = new MinecraftPrimitiveWriter(ms);
                    w.WriteByteArray(expected);
                    data = ms.ToArray();
                }
                using (MemoryStream ms = new MemoryStream(data))
                {
                    IMinecraftPrimitiveReader w = new MinecraftPrimitiveReader(ms);
                    byte[] d = w.ReadByteArray();

                    CollectionAssert.AreEqual(expected, d, "Массивы не совпадают");
                }


            }
        }
    }
}