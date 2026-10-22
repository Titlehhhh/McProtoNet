using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class TestInstanceBlockActionData : IProtocolType<TestInstanceBlockActionData>
{
    public string? Test { get; }
    public Vec3i Size { get; }
    public int Rotation { get; }
    public bool IgnoreEntities { get; }
    public int Status { get; }
    public NbtTag? ErrorMessage { get; }

    public TestInstanceBlockActionData(string? test, Vec3i size, int rotation, bool ignoreEntities, int status, NbtTag? errorMessage)
    {
        Test = test;
        Size = size;
        Rotation = rotation;
        IgnoreEntities = ignoreEntities;
        Status = status;
        ErrorMessage = errorMessage;
    }

    public static TestInstanceBlockActionData Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TestInstanceBlockActionData>(protocolVersion);
        string? test = null;
        if (reader.ReadBoolean())
            test = reader.ReadString();
        var size = reader.ReadType<Vec3i>(protocolVersion);
        var rotation = reader.ReadVarInt();
        var ignoreEntities = reader.ReadBoolean();
        var status = reader.ReadVarInt();
        NbtTag? errorMessage = null;
        if (reader.ReadBoolean())
            errorMessage = reader.ReadNbtTag(false)!;
        return new TestInstanceBlockActionData(test, size, rotation, ignoreEntities, status, errorMessage);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TestInstanceBlockActionData>(protocolVersion);
        writer.WriteBoolean(Test is not null);
        if (Test is { } testValue)
            writer.WriteString(testValue);
        writer.WriteType<Vec3i>(Size, protocolVersion);
        writer.WriteVarInt(Rotation);
        writer.WriteBoolean(IgnoreEntities);
        writer.WriteVarInt(Status);
        writer.WriteBoolean(ErrorMessage is not null);
        if (ErrorMessage is { } errorMessageValue)
            writer.WriteNbt(errorMessageValue);
    }
}
