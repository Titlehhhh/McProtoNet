using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoLib.API.IO;

namespace ProtoLib.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            MinecraftStream ms = new MinecraftStream();
            int val = 5;
            ms.WriteInt(val);
            int after = ms.ReadInt();

            Assert.AreEqual(val, after, "NoInt");
        }
    }
}