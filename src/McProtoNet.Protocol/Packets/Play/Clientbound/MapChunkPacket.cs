using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("MapChunk", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class MapChunkPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 736),
        new(751, 754),
        new(755, 756),
        new(757, 762),
        new(763, 763),
        new(764, MinecraftVersion.LatestProtocol),
    };

    public VFirst_736Fields? VFirst_736 { get; set; }
    public V751_754Fields? V751_754 { get; set; }
    public V755_756Fields? V755_756 { get; set; }
    public V757_762Fields? V757_762 { get; set; }
    public V763Fields? V763 { get; set; }
    public V764_LastFields? V764_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 736:
            {
                var fields = VFirst_736 ?? throw new InvalidOperationException("MapChunk VFirst_736 fields missing.");
                writer.WriteSignedInt(fields.X);
                writer.WriteSignedInt(fields.Z);
                writer.WriteBoolean(fields.GroundUp);
                writer.WriteBoolean(fields.IgnoreOldData);
                writer.WriteVarInt(fields.BitMap);
                writer.WriteNbtTag(fields.Heightmaps, protocolVersion);
                if (fields.GroundUp)
                {
                    WriteFixedIntArray(ref writer, fields.Biomes, 1024);
                }
                writer.WriteBuffer<VarInt>(fields.ChunkData);
                WriteNbtArray(ref writer, fields.BlockEntities, protocolVersion);
                return;
            }
            case >= 751 and <= 754:
            {
                var fields = V751_754 ?? throw new InvalidOperationException("MapChunk V751_754 fields missing.");
                writer.WriteSignedInt(fields.X);
                writer.WriteSignedInt(fields.Z);
                writer.WriteBoolean(fields.GroundUp);
                writer.WriteVarInt(fields.BitMap);
                writer.WriteNbtTag(fields.Heightmaps, protocolVersion);
                if (fields.GroundUp)
                {
                    writer.WriteVarInt(fields.Biomes.Length);
                    for (int i = 0; i < fields.Biomes.Length; i++)
                    {
                        writer.WriteVarInt(fields.Biomes[i]);
                    }
                }
                writer.WriteBuffer<VarInt>(fields.ChunkData);
                WriteNbtArray(ref writer, fields.BlockEntities, protocolVersion);
                return;
            }
            case >= 755 and <= 756:
            {
                var fields = V755_756 ?? throw new InvalidOperationException("MapChunk V755_756 fields missing.");
                writer.WriteSignedInt(fields.X);
                writer.WriteSignedInt(fields.Z);
                writer.WriteVarInt(fields.BitMap.Length);
                for (int i = 0; i < fields.BitMap.Length; i++)
                {
                    writer.WriteSignedLong(fields.BitMap[i]);
                }
                writer.WriteNbtTag(fields.Heightmaps, protocolVersion);
                writer.WriteVarInt(fields.Biomes.Length);
                for (int i = 0; i < fields.Biomes.Length; i++)
                {
                    writer.WriteVarInt(fields.Biomes[i]);
                }
                writer.WriteBuffer<VarInt>(fields.ChunkData);
                WriteNbtArray(ref writer, fields.BlockEntities, protocolVersion);
                return;
            }
            case >= 757 and <= 762:
            {
                var fields = V757_762 ?? throw new InvalidOperationException("MapChunk V757_762 fields missing.");
                writer.WriteSignedInt(fields.X);
                writer.WriteSignedInt(fields.Z);
                writer.WriteNbtTag(fields.Heightmaps, protocolVersion);
                writer.WriteBuffer<VarInt>(fields.ChunkData);
                WriteChunkBlockEntities(ref writer, fields.BlockEntities, protocolVersion);
                writer.WriteBoolean(fields.TrustEdges);
                WriteLightArrays(ref writer, fields.SkyLightMask, fields.BlockLightMask, fields.EmptySkyLightMask,
                    fields.EmptyBlockLightMask, fields.SkyLight, fields.BlockLight);
                return;
            }
            case 763:
            {
                var fields = V763 ?? throw new InvalidOperationException("MapChunk V763 fields missing.");
                writer.WriteSignedInt(fields.X);
                writer.WriteSignedInt(fields.Z);
                writer.WriteNbtTag(fields.Heightmaps, protocolVersion);
                writer.WriteBuffer<VarInt>(fields.ChunkData);
                WriteChunkBlockEntities(ref writer, fields.BlockEntities, protocolVersion);
                WriteLightArrays(ref writer, fields.SkyLightMask, fields.BlockLightMask, fields.EmptySkyLightMask,
                    fields.EmptyBlockLightMask, fields.SkyLight, fields.BlockLight);
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V764_Last ?? throw new InvalidOperationException("MapChunk V764_Last fields missing.");
                writer.WriteSignedInt(fields.X);
                writer.WriteSignedInt(fields.Z);
                writer.WriteAnonymousNbtTag(fields.Heightmaps, protocolVersion);
                writer.WriteBuffer<VarInt>(fields.ChunkData);
                WriteChunkBlockEntities(ref writer, fields.BlockEntities, protocolVersion);
                WriteLightArrays(ref writer, fields.SkyLightMask, fields.BlockLightMask, fields.EmptySkyLightMask,
                    fields.EmptyBlockLightMask, fields.SkyLight, fields.BlockLight);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.MapChunk), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 736:
            {
                int x = reader.ReadSignedInt();
                int z = reader.ReadSignedInt();
                bool groundUp = reader.ReadBoolean();
                bool ignoreOldData = reader.ReadBoolean();
                int bitMap = reader.ReadVarInt();
                NbtTag heightmaps = reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                int[]? biomes = groundUp ? ReadFixedIntArray(ref reader, 1024) : null;
                byte[] chunkData = reader.ReadBuffer(LengthFormat.VarInt);
                NbtTag[] blockEntities = ReadNbtArray(ref reader, protocolVersion);

                VFirst_736 = new VFirst_736Fields
                {
                    X = x,
                    Z = z,
                    GroundUp = groundUp,
                    IgnoreOldData = ignoreOldData,
                    BitMap = bitMap,
                    Heightmaps = heightmaps,
                    Biomes = biomes ?? Array.Empty<int>(),
                    ChunkData = chunkData,
                    BlockEntities = blockEntities
                };
                return;
            }
            case >= 751 and <= 754:
            {
                int x = reader.ReadSignedInt();
                int z = reader.ReadSignedInt();
                bool groundUp = reader.ReadBoolean();
                int bitMap = reader.ReadVarInt();
                NbtTag heightmaps = reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                int[] biomes = groundUp ? ReadVarIntArray(ref reader) : Array.Empty<int>();
                byte[] chunkData = reader.ReadBuffer(LengthFormat.VarInt);
                NbtTag[] blockEntities = ReadNbtArray(ref reader, protocolVersion);

                V751_754 = new V751_754Fields
                {
                    X = x,
                    Z = z,
                    GroundUp = groundUp,
                    BitMap = bitMap,
                    Heightmaps = heightmaps,
                    Biomes = biomes,
                    ChunkData = chunkData,
                    BlockEntities = blockEntities
                };
                return;
            }
            case >= 755 and <= 756:
            {
                int x = reader.ReadSignedInt();
                int z = reader.ReadSignedInt();
                long[] bitMap = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
                NbtTag heightmaps = reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                int[] biomes = ReadVarIntArray(ref reader);
                byte[] chunkData = reader.ReadBuffer(LengthFormat.VarInt);
                NbtTag[] blockEntities = ReadNbtArray(ref reader, protocolVersion);

                V755_756 = new V755_756Fields
                {
                    X = x,
                    Z = z,
                    BitMap = bitMap,
                    Heightmaps = heightmaps,
                    Biomes = biomes,
                    ChunkData = chunkData,
                    BlockEntities = blockEntities
                };
                return;
            }
            case >= 757 and <= 762:
            {
                int x = reader.ReadSignedInt();
                int z = reader.ReadSignedInt();
                NbtTag heightmaps = reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                byte[] chunkData = reader.ReadBuffer(LengthFormat.VarInt);
                ChunkBlockEntity[] blockEntities = ReadChunkBlockEntities(ref reader, protocolVersion);
                bool trustEdges = reader.ReadBoolean();
                ReadLightArrays(ref reader, out long[] skyLightMask, out long[] blockLightMask,
                    out long[] emptySkyLightMask, out long[] emptyBlockLightMask,
                    out byte[][] skyLight, out byte[][] blockLight);

                V757_762 = new V757_762Fields
                {
                    X = x,
                    Z = z,
                    Heightmaps = heightmaps,
                    ChunkData = chunkData,
                    BlockEntities = blockEntities,
                    TrustEdges = trustEdges,
                    SkyLightMask = skyLightMask,
                    BlockLightMask = blockLightMask,
                    EmptySkyLightMask = emptySkyLightMask,
                    EmptyBlockLightMask = emptyBlockLightMask,
                    SkyLight = skyLight,
                    BlockLight = blockLight
                };
                return;
            }
            case 763:
            {
                int x = reader.ReadSignedInt();
                int z = reader.ReadSignedInt();
                NbtTag heightmaps = reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                byte[] chunkData = reader.ReadBuffer(LengthFormat.VarInt);
                ChunkBlockEntity[] blockEntities = ReadChunkBlockEntities(ref reader, protocolVersion);
                ReadLightArrays(ref reader, out long[] skyLightMask, out long[] blockLightMask,
                    out long[] emptySkyLightMask, out long[] emptyBlockLightMask,
                    out byte[][] skyLight, out byte[][] blockLight);

                V763 = new V763Fields
                {
                    X = x,
                    Z = z,
                    Heightmaps = heightmaps,
                    ChunkData = chunkData,
                    BlockEntities = blockEntities,
                    SkyLightMask = skyLightMask,
                    BlockLightMask = blockLightMask,
                    EmptySkyLightMask = emptySkyLightMask,
                    EmptyBlockLightMask = emptyBlockLightMask,
                    SkyLight = skyLight,
                    BlockLight = blockLight
                };
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                int x = reader.ReadSignedInt();
                int z = reader.ReadSignedInt();
                NbtTag heightmaps = reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                byte[] chunkData = reader.ReadBuffer(LengthFormat.VarInt);
                ChunkBlockEntity[] blockEntities = ReadChunkBlockEntities(ref reader, protocolVersion);
                ReadLightArrays(ref reader, out long[] skyLightMask, out long[] blockLightMask,
                    out long[] emptySkyLightMask, out long[] emptyBlockLightMask,
                    out byte[][] skyLight, out byte[][] blockLight);

                V764_Last = new V764_LastFields
                {
                    X = x,
                    Z = z,
                    Heightmaps = heightmaps,
                    ChunkData = chunkData,
                    BlockEntities = blockEntities,
                    SkyLightMask = skyLightMask,
                    BlockLightMask = blockLightMask,
                    EmptySkyLightMask = emptySkyLightMask,
                    EmptyBlockLightMask = emptyBlockLightMask,
                    SkyLight = skyLight,
                    BlockLight = blockLight
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.MapChunk), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static void WriteFixedIntArray(ref MinecraftPrimitiveWriter writer, int[] values, int expectedLength)
    {
        if (values.Length != expectedLength)
        {
            throw new InvalidOperationException($"Expected {expectedLength} biomes entries, got {values.Length}.");
        }

        for (int i = 0; i < values.Length; i++)
        {
            writer.WriteSignedInt(values[i]);
        }
    }

    private static int[] ReadFixedIntArray(ref MinecraftPrimitiveReader reader, int count)
    {
        var values = new int[count];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = reader.ReadSignedInt();
        }
        return values;
    }

    private static int[] ReadVarIntArray(ref MinecraftPrimitiveReader reader)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<int>();
        }

        var values = new int[count];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = reader.ReadVarInt();
        }
        return values;
    }

    private static NbtTag[] ReadNbtArray(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<NbtTag>();
        }

        var tags = new NbtTag[count];
        for (int i = 0; i < tags.Length; i++)
        {
            tags[i] = reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("block entity missing");
        }
        return tags;
    }

    private static void WriteNbtArray(ref MinecraftPrimitiveWriter writer, NbtTag[] tags, int protocolVersion)
    {
        writer.WriteVarInt(tags.Length);
        for (int i = 0; i < tags.Length; i++)
        {
            writer.WriteNbtTag(tags[i], protocolVersion);
        }
    }

    private static ChunkBlockEntity[] ReadChunkBlockEntities(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<ChunkBlockEntity>();
        }

        var entities = new ChunkBlockEntity[count];
        for (int i = 0; i < entities.Length; i++)
        {
            byte packed = reader.ReadUnsignedByte();
            byte x = (byte)((packed >> 4) & 0x0F);
            byte z = (byte)(packed & 0x0F);
            short y = reader.ReadSignedShort();
            int type = reader.ReadVarInt();
            NbtTag? nbt = protocolVersion >= 764
                ? reader.ReadAnonOptionalNbtTag(protocolVersion)
                : reader.ReadOptionalNbtTag(protocolVersion);
            entities[i] = new ChunkBlockEntity
            {
                X = x,
                Z = z,
                Y = y,
                Type = type,
                NbtData = nbt
            };
        }

        return entities;
    }

    private static void WriteChunkBlockEntities(ref MinecraftPrimitiveWriter writer, ChunkBlockEntity[] entities,
        int protocolVersion)
    {
        writer.WriteVarInt(entities.Length);
        for (int i = 0; i < entities.Length; i++)
        {
            byte packed = (byte)(((entities[i].X & 0x0F) << 4) | (entities[i].Z & 0x0F));
            writer.WriteUnsignedByte(packed);
            writer.WriteSignedShort(entities[i].Y);
            writer.WriteVarInt(entities[i].Type);
            if (protocolVersion >= 764)
            {
                writer.WriteAnonOptionalNbtTag(entities[i].NbtData, protocolVersion);
            }
            else
            {
                writer.WriteOptionalNbtTag(entities[i].NbtData, protocolVersion);
            }
        }
    }

    private static void ReadLightArrays(ref MinecraftPrimitiveReader reader, out long[] skyLightMask,
        out long[] blockLightMask, out long[] emptySkyLightMask, out long[] emptyBlockLightMask,
        out byte[][] skyLight, out byte[][] blockLight)
    {
        skyLightMask = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
        blockLightMask = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
        emptySkyLightMask = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
        emptyBlockLightMask = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
        skyLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadArray<byte>(LengthFormat.VarInt));
        blockLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadArray<byte>(LengthFormat.VarInt));
    }

    private static void WriteLightArrays(ref MinecraftPrimitiveWriter writer, long[] skyLightMask, long[] blockLightMask,
        long[] emptySkyLightMask, long[] emptyBlockLightMask, byte[][] skyLight, byte[][] blockLight)
    {
        writer.WriteVarInt(skyLightMask.Length);
        for (int i = 0; i < skyLightMask.Length; i++)
        {
            writer.WriteSignedLong(skyLightMask[i]);
        }
        writer.WriteVarInt(blockLightMask.Length);
        for (int i = 0; i < blockLightMask.Length; i++)
        {
            writer.WriteSignedLong(blockLightMask[i]);
        }
        writer.WriteVarInt(emptySkyLightMask.Length);
        for (int i = 0; i < emptySkyLightMask.Length; i++)
        {
            writer.WriteSignedLong(emptySkyLightMask[i]);
        }
        writer.WriteVarInt(emptyBlockLightMask.Length);
        for (int i = 0; i < emptyBlockLightMask.Length; i++)
        {
            writer.WriteSignedLong(emptyBlockLightMask[i]);
        }
        writer.WriteVarInt(skyLight.Length);
        for (int i = 0; i < skyLight.Length; i++)
        {
            writer.WriteVarInt(skyLight[i].Length);
            writer.WriteBuffer(skyLight[i]);
        }
        writer.WriteVarInt(blockLight.Length);
        for (int i = 0; i < blockLight.Length; i++)
        {
            writer.WriteVarInt(blockLight[i].Length);
            writer.WriteBuffer(blockLight[i]);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_736Fields
    {
        public int X { get; set; }
        public int Z { get; set; }
        public bool GroundUp { get; set; }
        public bool IgnoreOldData { get; set; }
        public int BitMap { get; set; }
        public NbtTag Heightmaps { get; set; }
        public int[] Biomes { get; set; }
        public byte[] ChunkData { get; set; }
        public NbtTag[] BlockEntities { get; set; }
    }

    public struct V751_754Fields
    {
        public int X { get; set; }
        public int Z { get; set; }
        public bool GroundUp { get; set; }
        public int BitMap { get; set; }
        public NbtTag Heightmaps { get; set; }
        public int[] Biomes { get; set; }
        public byte[] ChunkData { get; set; }
        public NbtTag[] BlockEntities { get; set; }
    }

    public struct V755_756Fields
    {
        public int X { get; set; }
        public int Z { get; set; }
        public long[] BitMap { get; set; }
        public NbtTag Heightmaps { get; set; }
        public int[] Biomes { get; set; }
        public byte[] ChunkData { get; set; }
        public NbtTag[] BlockEntities { get; set; }
    }

    public struct V757_762Fields
    {
        public int X { get; set; }
        public int Z { get; set; }
        public NbtTag Heightmaps { get; set; }
        public byte[] ChunkData { get; set; }
        public ChunkBlockEntity[] BlockEntities { get; set; }
        public bool TrustEdges { get; set; }
        public long[] SkyLightMask { get; set; }
        public long[] BlockLightMask { get; set; }
        public long[] EmptySkyLightMask { get; set; }
        public long[] EmptyBlockLightMask { get; set; }
        public byte[][] SkyLight { get; set; }
        public byte[][] BlockLight { get; set; }
    }

    public struct V763Fields
    {
        public int X { get; set; }
        public int Z { get; set; }
        public NbtTag Heightmaps { get; set; }
        public byte[] ChunkData { get; set; }
        public ChunkBlockEntity[] BlockEntities { get; set; }
        public long[] SkyLightMask { get; set; }
        public long[] BlockLightMask { get; set; }
        public long[] EmptySkyLightMask { get; set; }
        public long[] EmptyBlockLightMask { get; set; }
        public byte[][] SkyLight { get; set; }
        public byte[][] BlockLight { get; set; }
    }

    public struct V764_LastFields
    {
        public int X { get; set; }
        public int Z { get; set; }
        public NbtTag Heightmaps { get; set; }
        public byte[] ChunkData { get; set; }
        public ChunkBlockEntity[] BlockEntities { get; set; }
        public long[] SkyLightMask { get; set; }
        public long[] BlockLightMask { get; set; }
        public long[] EmptySkyLightMask { get; set; }
        public long[] EmptyBlockLightMask { get; set; }
        public byte[][] SkyLight { get; set; }
        public byte[][] BlockLight { get; set; }
    }

    public struct ChunkBlockEntity
    {
        public byte X { get; set; }
        public byte Z { get; set; }
        public short Y { get; set; }
        public int Type { get; set; }
        public NbtTag? NbtData { get; set; }
    }
}
