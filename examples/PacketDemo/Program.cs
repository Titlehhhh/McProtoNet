using McProtoNet.Protocol;
using McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using McProtoNet.Protocol.Packets.Status.Serverbound;
using McProtoNet.Protocol.Packets.Login.Clientbound;
using McProtoNet.Protocol.Packets.Login.Serverbound;
using McProtoNet.Protocol.Packets.Configuration.Clientbound;
using McProtoNet.Protocol.Packets.Configuration.Serverbound;
using McProtoNet.Protocol.Packets.Play.Clientbound;
using McProtoNet.Serialization;

const int Version = 769;

Console.WriteLine("=== McProtoNet Packet Demo ===");
Console.WriteLine($"Protocol version: {Version}");
Console.WriteLine();

// --- Handshaking ---
Console.WriteLine("--- Handshaking ---");
{
    var pkt = new SetProtocolPacket
    {
        ProtocolVersion = Version,
        ServerHost = "localhost",
        ServerPort = 25565,
        NextState = 2
    };
    var buf = Serialize(pkt, Version);
    var pkt2 = new SetProtocolPacket();
    Deserialize(pkt2, buf, Version);
    Assert(pkt2.ProtocolVersion == Version, "SetProtocol.ProtocolVersion");
    Assert(pkt2.ServerHost == "localhost", "SetProtocol.ServerHost");
    Assert(pkt2.ServerPort == 25565, "SetProtocol.ServerPort");
    Assert(pkt2.NextState == 2, "SetProtocol.NextState");
    Console.WriteLine($"SetProtocolPacket OK (host={pkt2.ServerHost}, port={pkt2.ServerPort})");
    Console.WriteLine($"  Identifier: {SetProtocolPacket.Identifier}");
    Console.WriteLine($"  HexId: 0x{SetProtocolPacket.GetHexId(Version):X2}");
    Console.WriteLine($"  IsSupportedVersion({Version}): {SetProtocolPacket.IsSupportedVersion(Version)}");
}

// --- Status ---
Console.WriteLine();
Console.WriteLine("--- Status ---");
{
    var pingStart = new PingStartPacket();
    var buf = Serialize(pingStart, Version);
    var pingStart2 = new PingStartPacket();
    Deserialize(pingStart2, buf, Version);
    Console.WriteLine("PingStartPacket OK (no fields)");

    var ping = new McProtoNet.Protocol.Packets.Status.Serverbound.PingPacket { Time = 12345678L };
    var pingBuf = Serialize(ping, Version);
    var ping2 = new McProtoNet.Protocol.Packets.Status.Serverbound.PingPacket();
    Deserialize(ping2, pingBuf, Version);
    Assert(ping2.Time == 12345678L, "Status.PingPacket.Time");
    Console.WriteLine($"Status.PingPacket OK (time={ping2.Time})");
}

// --- Login ---
Console.WriteLine();
Console.WriteLine("--- Login ---");
{
    var loginStart = new LoginStartPacket
    {
        Username = "TestPlayer",
        V764_Last = new LoginStartPacket.V764_LastFields { PlayerUUID = Guid.NewGuid() }
    };
    var buf = Serialize(loginStart, Version);
    var loginStart2 = new LoginStartPacket();
    Deserialize(loginStart2, buf, Version);
    Assert(loginStart2.Username == "TestPlayer", "LoginStart.Username");
    Console.WriteLine($"LoginStartPacket OK (username={loginStart2.Username})");

    var compress = new CompressPacket { Threshold = 256 };
    var cBuf = Serialize(compress, Version);
    var compress2 = new CompressPacket();
    Deserialize(compress2, cBuf, Version);
    Assert(compress2.Threshold == 256, "Compress.Threshold");
    Console.WriteLine($"CompressPacket OK (threshold={compress2.Threshold})");
}

// --- Configuration ---
Console.WriteLine();
Console.WriteLine("--- Configuration ---");
{
    var finish = new McProtoNet.Protocol.Packets.Configuration.Clientbound.FinishConfigurationPacket
    {
        V764_Last = new McProtoNet.Protocol.Packets.Configuration.Clientbound.FinishConfigurationPacket.V764_LastFields { Container = 42 }
    };
    var buf = Serialize(finish, Version);
    var finish2 = new McProtoNet.Protocol.Packets.Configuration.Clientbound.FinishConfigurationPacket();
    Deserialize(finish2, buf, Version);
    Assert(finish2.V764_Last!.Value.Container == 42, "FinishConfiguration.Container");
    Console.WriteLine($"FinishConfigurationPacket (CB) OK (container={finish2.V764_Last!.Value.Container})");

    var finishSb = new McProtoNet.Protocol.Packets.Configuration.Serverbound.FinishConfigurationPacket
    {
        V764_Last = new McProtoNet.Protocol.Packets.Configuration.Serverbound.FinishConfigurationPacket.V764_LastFields { Container = 7 }
    };
    var sbBuf = Serialize(finishSb, Version);
    var finishSb2 = new McProtoNet.Protocol.Packets.Configuration.Serverbound.FinishConfigurationPacket();
    Deserialize(finishSb2, sbBuf, Version);
    Assert(finishSb2.V764_Last!.Value.Container == 7, "FinishConfiguration SB.Container");
    Console.WriteLine($"FinishConfigurationPacket (SB) OK (container={finishSb2.V764_Last!.Value.Container})");
}

// --- Play ---
Console.WriteLine();
Console.WriteLine("--- Play ---");
{
    long keepAliveId = 12345L;
    var ka = new 
        McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket 
        { KeepAliveId = keepAliveId };
    
    var buf = Serialize(ka, Version);
    var ka2 = new McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket();
    Deserialize(ka2, buf, Version);
    Assert(ka2.KeepAliveId == keepAliveId, "KeepAlive.KeepAliveId");
    Console.WriteLine($"Play.KeepAlivePacket (CB) OK (id={ka2.KeepAliveId})");
}

// --- PacketIdHelper ---
Console.WriteLine();
Console.WriteLine("--- PacketIdHelper ---");
{
    bool found = PacketIdHelper.TryGetPacketIdentifier(0x00, Version, PacketState.Handshaking, PacketDirection.Serverbound, out var id);
    Console.WriteLine($"TryGetPacketIdentifier(0x00, {Version}, Handshaking, Serverbound) = {found}, name={id.Name}");
    Assert(found, "TryGetPacketIdentifier found");
    Assert(id.Name == "SetProtocol", $"name == SetProtocol (was {id.Name})");

    bool foundRev = PacketIdHelper.TryGetPacketId(SetProtocolPacket.Identifier, Version, out int hexId);
    Console.WriteLine($"TryGetPacketId(SetProtocol, {Version}) = {foundRev}, hexId=0x{hexId:X2}");
    Assert(foundRev, "TryGetPacketId found");
    Assert(hexId == 0x00, $"hexId == 0x00 (was 0x{hexId:X2})");
}

// --- PacketFactory ---
Console.WriteLine();
Console.WriteLine("--- PacketFactory ---");
{
    bool created = PacketIdHelper.TryGetFactory(0x00, Version, PacketState.Handshaking, PacketDirection.Serverbound, out var factory);
    Assert(created && factory != null, "TryGetFactory found");
    var instance = factory!();
    Assert(instance is SetProtocolPacket, $"factory() returns SetProtocolPacket (was {instance.GetType().Name})");
    Console.WriteLine($"TryGetFactory(0x00, {Version}, Handshaking, Serverbound) -> {instance.GetType().Name}");
}

Console.WriteLine();
Console.WriteLine("All assertions passed!");

// --- Helpers ---
static byte[] Serialize(IPacket packet, int version)
{
    var writer = new MinecraftPrimitiveWriter();
    try
    {
        packet.Serialize(ref writer, version);
        return writer.WrittenSpan.ToArray();
    }
    finally
    {
        writer.Dispose();
    }
}

static void Deserialize(IPacket packet, byte[] data, int version)
{
    var reader = new MinecraftPrimitiveReader(data);
    packet.Deserialize(ref reader, version);
}

static void Assert(bool condition, string name)
{
    if (!condition)
        throw new Exception($"Assertion failed: {name}");
}
