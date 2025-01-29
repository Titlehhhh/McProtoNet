using McProtoNet.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace McProtoNet.Tests;

[TestClass]
public class MultiVersionTest
{
    [TestMethod]
    public void TestMultiVersion()
    {
        int version = 477;
        
    }

    [TestMethod]
    public void PacketFactoryInit()
    {
        PacketFactory.CreateClientboundPacket(340, 0x00, PacketState.Play);
    }
}