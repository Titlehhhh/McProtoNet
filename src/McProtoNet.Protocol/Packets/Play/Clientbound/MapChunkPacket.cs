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

    public int X { get; set; }
    public int Z { get; set; }
    public bool GroundUp { get; set; }
    public bool IgnoreOldData { get; set; }
    public int BitMap { get; set; }
    public long[] BitMapLong { get; set; } = Array.Empty<long>();
    public NbtTag Heightmaps { get; set; }
    public int[] Biomes { get; set; } = Array.Empty<int>();
    public byte[] ChunkData { get; set; } = Array.Empty<byte>();
    public NbtTag[] BlockEntitiesOld { get; set; } = Array.Empty<NbtTag>();
    public ChunkBlockEntity[] BlockEntities { get; set; } = Array.Empty<ChunkBlockEntity>();
    public bool TrustEdges { get; set; }
    public long[] SkyLightMask { get; set; } = Array.Empty<long>();
    public long[] BlockLightMask { get; set; } = Array.Empty<long>();
    public long[] EmptySkyLightMask { get; set; } = Array.Empty<long>();
    public long[] EmptyBlockLightMask { get; set; } = Array.Empty<long>();
    public byte[][] SkyLight { get; set; } = Array.Empty<byte[]>();
    public byte[][] BlockLight { get; set; } = Array.Empty<byte[]>();

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 736:
                writer.WriteSignedInt(X);
                writer.WriteSignedInt(Z);
                writer.WriteBoolean(GroundUp);
                writer.WriteBoolean(IgnoreOldData);
                writer.WriteVarInt(BitMap);
                writer.WriteNbtTag(Heightmaps, protocolVersion);
                if (GroundUp)
                {
                    WriteFixedIntArray(ref writer, Biomes, 1024);
                }
                writer.WriteBuffer<VarInt>(ChunkData);
                WriteNbtArray(ref writer, BlockEntitiesOld, protocolVersion);
                return;
            case >= 751 and <= 754:
                writer.WriteSignedInt(X);
                writer.WriteSignedInt(Z);
                writer.WriteBoolean(GroundUp);
                writer.WriteVarInt(BitMap);
                writer.WriteNbtTag(Heightmaps, protocolVersion);
                if (GroundUp)
                {
                    writer.WriteVarInt(Biomes.Length);
                    for (int i = 0; i < Biomes.Length; i++)
                    {
                        writer.WriteVarInt(Biomes[i]);
                    }
                }
                writer.WriteBuffer<VarInt>(ChunkData);
                WriteNbtArray(ref writer, BlockEntitiesOld, protocolVersion);
                return;
            case >= 755 and <= 756:
                writer.WriteSignedInt(X);
                writer.WriteSignedInt(Z);
                writer.WriteVarInt(BitMapLong.Length);
                for (int i = 0; i < BitMapLong.Length; i++)
                {
                    writer.WriteSignedLong(BitMapLong[i]);
                }
                writer.WriteNbtTag(Heightmaps, protocolVersion);
                writer.WriteVarInt(Biomes.Length);
                for (int i = 0; i < Biomes.Length; i++)
                {
                    writer.WriteVarInt(Biomes[i]);
                }
                writer.WriteBuffer<VarInt>(ChunkData);
                WriteNbtArray(ref writer, BlockEntitiesOld, protocolVersion);
                return;
            case >= 757 and <= 762:
                writer.WriteSignedInt(X);
                writer.WriteSignedInt(Z);
                writer.WriteNbtTag(Heightmaps, protocolVersion);
                writer.WriteBuffer<VarInt>(ChunkData);
                WriteChunkBlockEntities(ref writer, BlockEntities, protocolVersion);
                writer.WriteBoolean(TrustEdges);
                WriteLightArrays(ref writer, SkyLightMask, BlockLightMask, EmptySkyLightMask, EmptyBlockLightMask, SkyLight, BlockLight);
                return;
            case 763:
                writer.WriteSignedInt(X);
                writer.WriteSignedInt(Z);
                writer.WriteNbtTag(Heightmaps, protocolVersion);
                writer.WriteBuffer<VarInt>(ChunkData);
                WriteChunkBlockEntities(ref writer, BlockEntities, protocolVersion);
                WriteLightArrays(ref writer, SkyLightMask, BlockLightMask, EmptySkyLightMask, EmptyBlockLightMask, SkyLight, BlockLight);
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                writer.WriteSignedInt(X);
                writer.WriteSignedInt(Z);
                writer.WriteAnonymousNbtTag(Heightmaps, protocolVersion);
                writer.WriteBuffer<VarInt>(ChunkData);
                WriteChunkBlockEntities(ref writer, BlockEntities, protocolVersion);
                WriteLightArrays(ref writer, SkyLightMask, BlockLightMask, EmptySkyLightMask, EmptyBlockLightMask, SkyLight, BlockLight);
                return;
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
                X = reader.ReadSignedInt();
                Z = reader.ReadSignedInt();
                GroundUp = reader.ReadBoolean();
                IgnoreOldData = reader.ReadBoolean();
                BitMap = reader.ReadVarInt();
                Heightmaps = reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                Biomes = GroundUp ? ReadFixedIntArray(ref reader, 1024) : Array.Empty<int>();
                ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
                BlockEntitiesOld = ReadNbtArray(ref reader, protocolVersion);
                BlockEntities = Array.Empty<ChunkBlockEntity>();
                TrustEdges = false;
                SkyLightMask = Array.Empty<long>();
                BlockLightMask = Array.Empty<long>();
                EmptySkyLightMask = Array.Empty<long>();
                EmptyBlockLightMask = Array.Empty<long>();
                SkyLight = Array.Empty<byte[]>();
                BlockLight = Array.Empty<byte[]>();
                BitMapLong = Array.Empty<long>();
                return;
            case >= 751 and <= 754:
                X = reader.ReadSignedInt();
                Z = reader.ReadSignedInt();
                GroundUp = reader.ReadBoolean();
                BitMap = reader.ReadVarInt();
                Heightmaps = reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                Biomes = GroundUp ? ReadVarIntArray(ref reader) : Array.Empty<int>();
                ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
                BlockEntitiesOld = ReadNbtArray(ref reader, protocolVersion);
                BlockEntities = Array.Empty<ChunkBlockEntity>();
                IgnoreOldData = false;
                TrustEdges = false;
                SkyLightMask = Array.Empty<long>();
                BlockLightMask = Array.Empty<long>();
                EmptySkyLightMask = Array.Empty<long>();
                EmptyBlockLightMask = Array.Empty<long>();
                SkyLight = Array.Empty<byte[]>();
                BlockLight = Array.Empty<byte[]>();
                BitMapLong = Array.Empty<long>();
                return;
            case >= 755 and <= 756:
                X = reader.ReadSignedInt();
                Z = reader.ReadSignedInt();
                BitMapLong = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
                Heightmaps = reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                Biomes = ReadVarIntArray(ref reader);
                ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
                BlockEntitiesOld = ReadNbtArray(ref reader, protocolVersion);
                BlockEntities = Array.Empty<ChunkBlockEntity>();
                GroundUp = true;
                IgnoreOldData = false;
                BitMap = 0;
                TrustEdges = false;
                SkyLightMask = Array.Empty<long>();
                BlockLightMask = Array.Empty<long>();
                EmptySkyLightMask = Array.Empty<long>();
                EmptyBlockLightMask = Array.Empty<long>();
                SkyLight = Array.Empty<byte[]>();
                BlockLight = Array.Empty<byte[]>();
                return;
            case >= 757 and <= 762:
                X = reader.ReadSignedInt();
                Z = reader.ReadSignedInt();
                Heightmaps = reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
                BlockEntities = ReadChunkBlockEntities(ref reader, protocolVersion);
                TrustEdges = reader.ReadBoolean();
                ReadLightArrays(ref reader, out long[] skyLightMask, out long[] blockLightMask, out long[] emptySkyLightMask, out long[] emptyBlockLightMask, out byte[][] skyLight, out byte[][] blockLight);
                SkyLightMask = skyLightMask;
                BlockLightMask = blockLightMask;
                EmptySkyLightMask = emptySkyLightMask;
                EmptyBlockLightMask = emptyBlockLightMask;
                SkyLight = skyLight;
                BlockLight = blockLight;
                GroundUp = true;
                IgnoreOldData = false;
                BitMap = 0;
                Biomes = Array.Empty<int>();
                BlockEntitiesOld = Array.Empty<NbtTag>();
                BitMapLong = Array.Empty<long>();
                return;
            case 763:
                X = reader.ReadSignedInt();
                Z = reader.ReadSignedInt();
                Heightmaps = reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
                BlockEntities = ReadChunkBlockEntities(ref reader, protocolVersion);
                ReadLightArrays(ref reader, out long[] skyLightMask, out long[] blockLightMask, out long[] emptySkyLightMask, out long[] emptyBlockLightMask, out byte[][] skyLight, out byte[][] blockLight);
                SkyLightMask = skyLightMask;
                BlockLightMask = blockLightMask;
                EmptySkyLightMask = emptySkyLightMask;
                EmptyBlockLightMask = emptyBlockLightMask;
                SkyLight = skyLight;
                BlockLight = blockLight;
                GroundUp = true;
                IgnoreOldData = false;
                BitMap = 0;
                Biomes = Array.Empty<int>();
                BlockEntitiesOld = Array.Empty<NbtTag>();
                TrustEdges = false;
                BitMapLong = Array.Empty<long>();
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                X = reader.ReadSignedInt();
                Z = reader.ReadSignedInt();
                Heightmaps = reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("heightmaps missing");
                ChunkData = reader.ReadBuffer(LengthFormat.VarInt);
                BlockEntities = ReadChunkBlockEntities(ref reader, protocolVersion);
                ReadLightArrays(ref reader, out long[] skyLightMask, out long[] blockLightMask, out long[] emptySkyLightMask, out long[] emptyBlockLightMask, out byte[][] skyLight, out byte[][] blockLight);
                SkyLightMask = skyLightMask;
                BlockLightMask = blockLightMask;
                EmptySkyLightMask = emptySkyLightMask;
                EmptyBlockLightMask = emptyBlockLightMask;
                SkyLight = skyLight;
                BlockLight = blockLight;
                GroundUp = true;
                IgnoreOldData = false;
                BitMap = 0;
                Biomes = Array.Empty<int>();
                BlockEntitiesOld = Array.Empty<NbtTag>();
                TrustEdges = false;
                BitMapLong = Array.Empty<long>();
                return;
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
            entities[i] = new ChunkBlockEntity(x, z, y, type, nbt);
        }

        return entities;
    }

    private static void WriteChunkBlockEntities(ref MinecraftPrimitiveWriter writer, ChunkBlockEntity[] entities, int protocolVersion)
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

    private static void ReadLightArrays(ref MinecraftPrimitiveReader reader, out long[] skyLightMask, out long[] blockLightMask, out long[] emptySkyLightMask, out long[] emptyBlockLightMask, out byte[][] skyLight, out byte[][] blockLight)
    {
        skyLightMask = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
        blockLightMask = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
        emptySkyLightMask = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
        emptyBlockLightMask = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
        skyLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadArray<byte>(LengthFormat.VarInt));
        blockLight = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadArray<byte>(LengthFormat.VarInt));
    }

    private static void WriteLightArrays(ref MinecraftPrimitiveWriter writer, long[] skyLightMask, long[] blockLightMask, long[] emptySkyLightMask, long[] emptyBlockLightMask, byte[][] skyLight, byte[][] blockLight)
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


}