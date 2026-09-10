using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol;

[ProtocolSupport(757, MinecraftVersion.LatestProtocol)]
public sealed partial class ChunkBlockEntity : IProtocolType<ChunkBlockEntity>
{
    public int PackedXZ { get; }
    public int Y { get; }
    public int Type { get; }
    public NbtTag? NbtData { get; }

    public ChunkBlockEntity(int packedXZ, int y, int type, NbtTag? nbtData)
    {
        PackedXZ = packedXZ;
        Y = y;
        Type = type;
        NbtData = nbtData;
    }

    public static ChunkBlockEntity Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBlockEntity>(protocolVersion);
        if (protocolVersion >= 757 && protocolVersion <= 763)
        {
            var packedXZ = reader.ReadUnsignedByte();
            var y = reader.ReadSignedShort();
            var type = reader.ReadVarInt();
            NbtTag? nbtData = null;
            if (reader.ReadBoolean())
                nbtData = reader.ReadNbtTag(true)!;
            return new ChunkBlockEntity(packedXZ, y, type, nbtData);
        }

        if (protocolVersion >= 764)
        {
            var packedXZ = reader.ReadUnsignedByte();
            var y = reader.ReadSignedShort();
            var type = reader.ReadVarInt();
            NbtTag? nbtData = null;
            if (reader.ReadBoolean())
                nbtData = reader.ReadNbtTag(false)!;
            return new ChunkBlockEntity(packedXZ, y, type, nbtData);
        }

        throw new System.NotSupportedException($"ChunkBlockEntity has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBlockEntity>(protocolVersion);
        if (protocolVersion >= 757 && protocolVersion <= 763)
        {
            writer.WriteUnsignedByte((byte)PackedXZ);
            writer.WriteSignedShort((short)Y);
            writer.WriteVarInt(Type);
            writer.WriteBoolean(NbtData is not null);
            if (NbtData is { } nbtDataValue)
                writer.WriteNbt(nbtDataValue, true);
            return;
        }

        if (protocolVersion >= 764)
        {
            writer.WriteUnsignedByte((byte)PackedXZ);
            writer.WriteSignedShort((short)Y);
            writer.WriteVarInt(Type);
            writer.WriteBoolean(NbtData is not null);
            if (NbtData is { } nbtDataValue)
                writer.WriteNbt(nbtDataValue);
            return;
        }

        throw new System.NotSupportedException($"ChunkBlockEntity has no wire layout for protocol version {protocolVersion}.");
    }
}
