using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.test_instance_block_action", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Position", "Position")]
[PacketField("Action", "int")]
[PacketField("Data", "TestInstanceBlockActionData")]
public sealed partial record TestInstanceBlockActionPacket(Position Position, int Action, TestInstanceBlockActionData Data) : IPacket<TestInstanceBlockActionPacket>, IPacket
{
    public static TestInstanceBlockActionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TestInstanceBlockActionPacket>(protocolVersion);
        var position = reader.ReadType<Position>(protocolVersion);
        var action = reader.ReadVarInt();
        string? dataTest = null;
        if (reader.ReadBoolean())
            dataTest = reader.ReadString();
        var dataSize = reader.ReadType<Vec3i>(protocolVersion);
        var dataRotation = reader.ReadVarInt();
        var dataIgnoreEntities = reader.ReadBoolean();
        var dataStatus = reader.ReadVarInt();
        NbtTag? dataErrorMessage = null;
        if (reader.ReadBoolean())
            dataErrorMessage = reader.ReadNbtTag(false)!;
        var data = new TestInstanceBlockActionData(dataTest, dataSize, dataRotation, dataIgnoreEntities, dataStatus, dataErrorMessage);
        return new TestInstanceBlockActionPacket(position, action, data);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TestInstanceBlockActionPacket>(protocolVersion);
        writer.WriteType<Position>(Position, protocolVersion);
        writer.WriteVarInt(Action);
        TestInstanceBlockActionData data = Data;
        writer.WriteBoolean(data.Test is not null);
        if (data.Test is { } testValue)
            writer.WriteString(testValue);
        writer.WriteType<Vec3i>(data.Size, protocolVersion);
        writer.WriteVarInt(data.Rotation);
        writer.WriteBoolean(data.IgnoreEntities);
        writer.WriteVarInt(data.Status);
        writer.WriteBoolean(data.ErrorMessage is not null);
        if (data.ErrorMessage is { } errorMessageValue)
            writer.WriteNbt(errorMessageValue);
    }

    public static PacketIdentity Identity => new("play.toServer.test_instance_block_action", "TestInstanceBlockAction", PacketPhase.Play, PacketDirection.Serverbound, 62);

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
