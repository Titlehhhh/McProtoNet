using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using CommandLine;
using DotNext;
using DotNext.Buffers;
using McProtoNet.Serialization;

namespace McProtoNet.Benchmark;

[MemoryDiagnoser]
public class ParseChunksBenchmarks
{
    private byte[] Data;

    [Params(true, false)] public bool WithSIMD { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        Data = File.ReadAllBytes("Test.bin");
    }

    private Chunk? ReadChunk(scoped ref MinecraftPrimitiveReader reader)
    {
        byte bitsPerEntry = reader.ReadUnsignedByte();

        if (bitsPerEntry == 0)
        {
            ushort blockId = (ushort)reader.ReadVarInt();
            Block block = new(blockId);
            reader.ReadVarInt();
            Chunk chunk = new();
            return chunk;
        }
        else
        {
            bool usePalette = bitsPerEntry <= 8;

            if (bitsPerEntry < 4) bitsPerEntry = 4;


            uint valueMask = (uint)((1 << bitsPerEntry) - 1);

            int paletteLength = usePalette ? reader.ReadVarInt() : 0;

            Span<uint> palette = paletteLength < 256 ? stackalloc uint[paletteLength] : new uint[paletteLength];
            for (int i = 0; i < paletteLength; i++)
                palette[i] = (uint)reader.ReadVarInt();


            //dict[palette.Length] = dict.TryGetValue(palette.Length, out var gg) ? gg + 1 : 1;


            int a = reader.ReadVarInt();


            Chunk chunk = new();
            if (WithSIMD && bitsPerEntry == 4 && Vector128.IsHardwareAccelerated)
            {
                Vectorized(ref reader, chunk, palette);
            }
            else
            {
                Span<byte> entryDataByte = stackalloc byte[8];
                Span<long> entryDataLong = MemoryMarshal.Cast<byte, long>(entryDataByte);


                int startOffset = 64;

                for (int blockY = 0; blockY < 16; blockY++)
                {
                    for (int blockZ = 0; blockZ < 16; blockZ++)
                    {
                        for (int blockX = 0; blockX < 16; blockX++)
                        {
                            startOffset += bitsPerEntry;
                            if (startOffset > 64 - bitsPerEntry)
                            {
                                startOffset = 0;
                                reader.Read(entryDataByte);
                                entryDataByte.Reverse();
                            }


                            uint blockId = (uint)(entryDataLong[0] >> startOffset) & valueMask;

                            if (usePalette)
                            {
                                if (paletteLength <= blockId)
                                {
                                    int blockNumber = (blockY * 16 + blockZ) * 16 + blockX;
                                    throw new IndexOutOfRangeException(String.Format(
                                        "Block ID {0} is outside Palette range 0-{1}! (bitsPerBlock: {2}, blockNumber: {3})",
                                        blockId,
                                        paletteLength - 1,
                                        bitsPerEntry,
                                        blockNumber));
                                }

                                blockId = palette[(int)blockId];
                            }

                            Block block = new((ushort)blockId);
                            chunk.SetFast(blockX, blockY, blockZ, block);
                        }
                    }
                }
            }

            return chunk;
        }
    }

    private static readonly MemoryAllocator<long> s_allocator = ArrayPool<long>.Shared.ToAllocator();

    private unsafe void Vectorized(ref MinecraftPrimitiveReader reader, Chunk chunk, scoped Span<uint> palette)
    {
        const int BytesPerVector = 16;
        const int VectorsPerChunk = 2048 / BytesPerVector;


        ReadOnlySpan<byte> bytes = reader.Read(2048);

        fixed (byte* pSource = &MemoryMarshal.GetReference(bytes))
        fixed (uint* pPalette = &MemoryMarshal.GetReference(palette))
        {
            byte* current = pSource;
            for (int i = 0; i < VectorsPerChunk; i += 4)
            {
                LoadAndSplit(current, pPalette, chunk);
                current += BytesPerVector;
                LoadAndSplit(current, pPalette, chunk);
                current += BytesPerVector;
                LoadAndSplit(current, pPalette, chunk);
                current += BytesPerVector;
                LoadAndSplit(current, pPalette, chunk);
                current += BytesPerVector;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<byte> Low(Vector128<byte> vector)
    {
        return vector & Vector128.Create((byte)0x0F);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<byte> High(Vector128<byte> vector)
    {
        return vector >>> 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe Vector128<byte> LoadBigEndian(byte* source)
    {
        var vec = Vector128.Load(source);
        return Vector128.Shuffle(vec, Vector128.Create((byte)7, 6, 5, 4, 3, 2, 1, 0, 15, 14, 13, 12, 11, 10, 9, 8));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void LoadAndSplit(byte* source, uint* palette, Chunk chunk)
    {
        var vec = LoadBigEndian(source);
        //var (lo, hi) = SplitNibbles(vec);
        ProcessVector(Low(vec), palette, chunk);
        ProcessVector(High(vec), palette, chunk);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (Vector128<byte> Lo, Vector128<byte> Hi) SplitNibbles(Vector128<byte> vector)
    {
        var lo = vector & Vector128.Create((byte)0x0F);
        var hi = vector >>> 4;
        return (lo, hi);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void ProcessVector(in Vector128<byte> vector, uint* palette, Chunk chunk)
    {
        byte* indices = (byte*)Unsafe.AsPointer(ref Unsafe.AsRef(in vector));

        // Преобразуем индексы в блоки за один проход
        for (int i = 0; i < 16; i++)
        {
            uint blockId = palette[indices[i]];
            // Реальная логика размещения блоков должна использовать координаты
            chunk.SetFast(i, 0, 0, new Block((ushort)blockId));
        }
    }

    [Benchmark]
    public void ParseChunks()
    {
        scoped MinecraftPrimitiveReader reader = new MinecraftPrimitiveReader(Data.AsSpan());

        int chunkColumnSize = 16;


        for (int chunkY = 0; chunkY < chunkColumnSize; chunkY++)
        {
            var lastChunkY = chunkColumnSize - 1;

            int blockCnt = reader.ReadSignedShort();
            Chunk? chunk = ReadChunk(ref reader);

            byte bitsPerEntryBiome = reader.ReadUnsignedByte();
            if (bitsPerEntryBiome == 0)
            {
                reader.ReadVarInt();
                reader.ReadVarInt();
            }
            else
            {
                if (bitsPerEntryBiome <= 3)
                {
                    int paletteLength = reader.ReadVarInt();
                    for (int i = 0; i < paletteLength; i++)
                        reader.ReadVarInt();
                }

                int dataArrayLength = reader.ReadVarInt();
                reader.Advance(dataArrayLength * 8);
            }
        }
    }
}

public struct Block
{
    public Block(ushort id)
    {
    }
}

public class World
{
}

public class Chunk
{
    public const int SizeX = 16;
    public const int SizeY = 16;
    public const int SizeZ = 16;
    private readonly Block[] blocks = new Block[SizeY * SizeZ * SizeX];


    public void SetFast(int x, int y, int z, Block block)
    {
        blocks[(y << 8) | (z << 4) | x] = block;
    }
}