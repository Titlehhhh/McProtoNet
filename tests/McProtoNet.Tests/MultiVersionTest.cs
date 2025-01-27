using Microsoft.VisualStudio.TestTools.UnitTesting;
using CPlay = McProtoNet.Protocol.ClientboundPackets.Play;
using SPlay = McProtoNet.Protocol.ServerboundPackets.Play;

namespace McProtoNet.Tests;

[TestClass]
public class MultiVersionTest
{
    [TestMethod]
    public void TestMultiVersion()
    {
        int version = 477;
        
        Assert.IsTrue(SPlay.AbilitiesPacket.V340_710.SupportedVersion(version), "AbilitiesPacket");
        Assert.IsTrue(CPlay.AbilitiesPacket.SupportedVersion(version),
            "CPlay.AbilitiesPacket.SupportedVersion(version)");
        
        Assert.IsFalse(SPlay.AbilitiesPacket.V734_769.SupportedVersion(version), "AbilitiesPacket");
    }
}