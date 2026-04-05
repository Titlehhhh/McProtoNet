using McProtoNet.Protocol;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using McProtoNet.Serialization;

const int Version = 769;

Console.WriteLine("=== McProtoNet Serialization Demo ===");
Console.WriteLine();

// --- MinecraftPrimitiveWriter / Reader ---
Console.WriteLine("--- Primitives ---");
{
    var writer = new MinecraftPrimitiveWriter();
    writer.WriteVarInt(300);
    writer.WriteString("Hello, Minecraft!");
    writer.WriteBoolean(true);
    writer.WriteSignedLong(long.MaxValue);
    writer.WriteFloat(3.14f);
    writer.WriteUUID(new Guid("12345678-1234-1234-1234-123456789abc"));

    var reader = new MinecraftPrimitiveReader(writer.WrittenMemory);
    Console.WriteLine($"VarInt:  {reader.ReadVarInt()}");
    Console.WriteLine($"String:  {reader.ReadString()}");
    Console.WriteLine($"Boolean: {reader.ReadBoolean()}");
    Console.WriteLine($"Long:    {reader.ReadSignedLong()}");
    Console.WriteLine($"Float:   {reader.ReadFloat()}");
    Console.WriteLine($"UUID:    {reader.ReadUUID()}");
    Console.WriteLine($"Total bytes written: {writer.WrittenSpan.Length}");
}

// --- GetWrittenMemory: pooled copy ---
Console.WriteLine();
Console.WriteLine("--- GetWrittenMemory (pooled) ---");
{
    var writer = new MinecraftPrimitiveWriter();
    writer.WriteVarInt(42);
    writer.WriteString("test");

    using var mem = writer.GetWrittenMemory();
    Console.WriteLine($"Pooled copy: {mem.Length} bytes");
    var reader = new MinecraftPrimitiveReader(mem.Memory);
    Console.WriteLine($"VarInt: {reader.ReadVarInt()}, String: {reader.ReadString()}");
}

// --- PacketMarshaller: KeepAlive (Play Serverbound) ---
Console.WriteLine();
Console.WriteLine("--- PacketMarshaller: KeepAlive SB ---");
{
    // Note: packets currently have no explicit namespace — reference directly
    var ka = new KeepAlivePacket { KeepAliveId = 98765L };
    using var buf = PacketMarshaller.Serialize(ka, Version);
    Console.WriteLine($"Serialized: {buf.Length} bytes");

    var ka2 = new KeepAlivePacket();
    PacketMarshaller.Deserialize(ka2, buf.Memory, Version);
    Console.WriteLine($"KeepAliveId: {ka2.KeepAliveId}");
}

// --- PacketIdHelper ---
Console.WriteLine();
Console.WriteLine("--- PacketIdHelper ---");
{
    if (PacketIdHelper.TryGetPacketIdentifier(0x00, Version, PacketState.Handshaking, PacketDirection.Serverbound, out var id))
        Console.WriteLine($"0x00 Handshaking/SB → {id.Name}");

    if (PacketIdHelper.TryGetPacketId(SetProtocolPacket.Identifier, Version, out int hexId))
        Console.WriteLine($"SetProtocol → 0x{hexId:X2}");
}

// --- PacketFactory ---
Console.WriteLine();
Console.WriteLine("--- PacketFactory ---");
{
    if (PacketIdHelper.TryGetFactory(0x00, Version, PacketState.Handshaking, PacketDirection.Serverbound, out var factory))
    {
        var instance = factory!();
        Console.WriteLine($"Factory(0x00, Handshaking, SB) → {instance.GetType().Name}");
    }
}

Console.WriteLine();
Console.WriteLine("Done.");
