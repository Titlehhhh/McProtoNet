using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public sealed partial class Heightmap : IProtocolType<Heightmap>
{
    public HeightmapType Type { get; }
    public long[] Data { get; }

    public Heightmap(HeightmapType type, long[] data)
    {
        Type = type;
        Data = data;
    }

    public static Heightmap Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Heightmap>(protocolVersion);
        if (protocolVersion >= 770 && protocolVersion <= 770)
        {
            var type = new HeightmapType((int)reader.ReadVarInt());
            int dataCount = reader.ReadVarInt();
            var data = new long[dataCount];
            for (int i = 0; i < data.Length; i++)
                data[i] = reader.ReadSignedLong();
            return new Heightmap(type, data);
        }

        if (protocolVersion >= 771)
        {
            var type = reader.ReadType<HeightmapType>(protocolVersion);
            int dataCount = reader.ReadVarInt();
            var data = new long[dataCount];
            for (int i = 0; i < data.Length; i++)
                data[i] = reader.ReadSignedLong();
            return new Heightmap(type, data);
        }

        throw new System.NotSupportedException($"Heightmap has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Heightmap>(protocolVersion);
        if (protocolVersion >= 770 && protocolVersion <= 770)
        {
            writer.WriteVarInt((int)Type.Value);
            writer.WriteVarInt(Data.Length);
            foreach (var dataItem in Data)
                writer.WriteSignedLong(dataItem);
            return;
        }

        if (protocolVersion >= 771)
        {
            writer.WriteType<HeightmapType>(Type, protocolVersion);
            writer.WriteVarInt(Data.Length);
            foreach (var dataItem in Data)
                writer.WriteSignedLong(dataItem);
            return;
        }

        throw new System.NotSupportedException($"Heightmap has no wire layout for protocol version {protocolVersion}.");
    }
}
