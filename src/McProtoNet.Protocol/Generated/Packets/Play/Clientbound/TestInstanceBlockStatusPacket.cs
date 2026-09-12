using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.test_instance_block_status", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Status", "NbtTag")]
[PacketField("Size", "Vec3i?")]
public sealed partial record TestInstanceBlockStatusPacket(NbtTag Status, Vec3i? Size) : IPacket<TestInstanceBlockStatusPacket>, IPacket
{
    public static TestInstanceBlockStatusPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TestInstanceBlockStatusPacket>(protocolVersion);
        var status = reader.ReadNbtTag(false)!;
        Vec3i? size = null;
        if (reader.ReadBoolean())
            size = reader.ReadType<Vec3i>(protocolVersion);
        return new TestInstanceBlockStatusPacket(status, size);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TestInstanceBlockStatusPacket>(protocolVersion);
        writer.WriteNbt(Status);
        writer.WriteBoolean(Size is not null);
        if (Size is { } sizeValue)
            writer.WriteType<Vec3i>(sizeValue, protocolVersion);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Status");
        Status.WriteJson(writer);
        if (Size is { } sizeValue)
        {
            writer.WritePropertyName("Size");
            sizeValue.WriteJson(writer);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.test_instance_block_status", "TestInstanceBlockStatus", PacketPhase.Play, PacketDirection.Clientbound, 115);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
