using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using DotNext;
using DotNext.Buffers;
using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Packets.Configuration.Serverbound;
using McProtoNet.Serialization;
using CPlay = McProtoNet.Protocol.Packets.Play.Clientbound;
using SPlay = McProtoNet.Protocol.Packets.Play.Serverbound;

namespace SampleBotCSharp;

public class Bot
{
    private MinecraftVersion _version = MinecraftVersion.V1_21_4;

    //public static ConcurrentDictionary<int, int> bits = new();

    private MinecraftClient _client;

    public async Task Start()
    {
        _client = new MinecraftClient(new MinecraftClientStartOptions()
        {
            ConnectTimeout = TimeSpan.FromSeconds(5),
            Host = "title-kde",
            Port = 25565,
            WriteTimeout = TimeSpan.FromSeconds(5),
            ReadTimeout = TimeSpan.FromSeconds(5),
            Version = (int)_version
        });

        await _client.ConnectAsync();
        await _client.Login("TestBot");
        _ = Task.Run(async () =>
        {
            try
            {
                Console.WriteLine("Start Play");
                await foreach (var packet in _client.OnAllPackets(PacketState.Play))
                {
                    HandlePlayPacket(packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read packets: {ex}");
                throw;
            }
            finally
            {
                Console.WriteLine("Stop Play");
            }
        });
    }

    private void HandlePlayPacket(IServerPacket packet)
    {
        try
        {
            if (packet is CPlay.KeepAlivePacket keepAlive)
            {
                Console.WriteLine("KeepAlive");
                _client.SendPacket(new SPlay.KeepAlivePacket()
                {
                    KeepAliveId = keepAlive.KeepAliveId
                });
            }
            else if (packet is CPlay.MapChunkPacket mapChunkPacket)
            {
                File.WriteAllBytes("Test.bin", mapChunkPacket.ChunkData);
                
                //Console.WriteLine($"Load chunk: X: {mapChunkPacket.X} Z: {mapChunkPacket.Z}");
                HandleChunk(mapChunkPacket);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private Chunk? ReadChunk(scoped ref MinecraftPrimitiveReader reader)
    {
        // read Block states (Type: Paletted Container)
        byte bitsPerEntry = reader.ReadUnsignedByte();

        // 1.18(1.18.1) add a pattle named "Single valued" to replace the vertical strip bitmask in the old
        if (bitsPerEntry == 0)
        {
            // Palettes: Single valued - 1.18(1.18.1) and above
            ushort blockId = (ushort)reader.ReadVarInt();
            Block block = new(blockId);

            reader.ReadVarInt(); // Data Array Length will be zero

            // Empty chunks will not be stored

            //if (block.Type == Material.Air)
            //    return null;

            // Warning: If you need to support modification of block data, you need to create 4096 objects here
            Chunk chunk = new();
            //for (int blockY = 0; blockY < Chunk.SizeY; blockY++)
            //    for (int blockZ = 0; blockZ < Chunk.SizeZ; blockZ++)
            //        for (int blockX = 0; blockX < Chunk.SizeX; blockX++)
            //            chunk.SetWithoutCheck(blockX, blockY, blockZ, block);
            return chunk;
        }
        else
        {
            // Palettes: Indirect or Direct
            bool usePalette = bitsPerEntry <= 8;


            // Indirect Mode: For block states with bits per entry <= 4, 4 bits are used to represent a block.
            if (bitsPerEntry < 4) bitsPerEntry = 4;
            if (bitsPerEntry == 4)
            {
                Environment.FailFast("asd");
            }

            // Direct Mode: Bit mask covering bitsPerEntry bits
            // EG, if bitsPerEntry = 5, valueMask = 00011111 in binary
            uint valueMask = (uint)((1 << bitsPerEntry) - 1);
            

            int paletteLength = usePalette ? reader.ReadVarInt() : 0; // Assume zero when length is absent

            Span<uint> palette = paletteLength < 256 ? stackalloc uint[paletteLength] : new uint[paletteLength];
            for (int i = 0; i < paletteLength; i++)
                palette[i] = (uint)reader.ReadVarInt();

            Console.WriteLine($"Pallete length: {paletteLength}");

            //// Block IDs are packed in the array of 64-bits integers
            reader.ReadVarInt(); // Entry length


            Span<byte> entryDataByte = stackalloc byte[8];
            Span<long>
                entryDataLong = MemoryMarshal.Cast<byte, long>(entryDataByte);

            Chunk chunk = new();
            int startOffset = 64; // Read the first data immediately

            for (int blockY = 0; blockY < 16; blockY++)
            {
                for (int blockZ = 0; blockZ < 16; blockZ++)
                {
                    for (int blockX = 0; blockX < 16; blockX++)
                    {
                        startOffset += bitsPerEntry;
                        // Calculate location of next block ID inside the array of Longs
                        if (startOffset > 64 - bitsPerEntry)
                        {
                            // In MC 1.16+, padding is applied to prevent overlapping between Longs:
                            // [     LONG INTEGER     ][     LONG INTEGER     ]
                            // [Block][Block][Block]XXX[Block][Block][Block]XXX

                            startOffset = 0;
                            reader.Read(entryDataByte);
                            entryDataByte.Reverse();
                        }

                        if (bitsPerEntry == 4)
                        {
                            Debugger.Break();
                        }
                        uint blockId = (uint)(entryDataLong[0] >> startOffset) & valueMask;

                        // Map small IDs to actual larger block IDs
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

                        // NOTICE: In the future a single ushort may not store the entire block id;
                        // the Block class may need to change if block state IDs go beyond 65535
                        Block block = new((ushort)blockId);

                        // We have our block, save the block into the chunk
                        //chunk.SetWithoutCheck(blockX, blockY, blockZ, block);
                    }
                }
            }


            return chunk;
        }
    }

    private static readonly MemoryAllocator<long> s_allocator = ArrayPool<long>.Shared.ToAllocator();

    // if bitsPerBlock = 4
    private void Fast(ref MinecraftPrimitiveReader reader)
    {
        MemoryOwner<long> longs = s_allocator.AllocateExactly(256);
        Span<long> span = longs.Span;
        try
        {
            reader.ReadArrayInt64BigEndian(span);

            Span<byte> bytes = MemoryMarshal.Cast<long, byte>(span);

            ref byte pBytes = ref MemoryMarshal.GetReference(bytes);

            nuint elementOffset = 0;
            Vector128<byte> mask = Vector128.CreateScalar((byte)0b_0000_1111);
            
            nuint oneVectorAwayFromEnd = (nuint)(bytes.Length - Vector128<byte>.Count);
            for (; elementOffset <= oneVectorAwayFromEnd; elementOffset += (nuint)Vector128<byte>.Count)
            {
                Vector128<byte> loaded = Vector128.LoadUnsafe(ref pBytes, elementOffset);
                Vector128<byte> second =  loaded & mask;
                Vector128<byte> first = loaded >> 4 & mask;
            }
            
        }
        finally
        {
            longs.Dispose();
        }
    }

    private void HandleChunk(CPlay.MapChunkPacket mapChunkPacket)
    {
        scoped MinecraftPrimitiveReader reader = new MinecraftPrimitiveReader(mapChunkPacket.ChunkData.AsSpan());

        World world = null;

        int chunkColumnSize = 16; 


        for (int chunkY = 0; chunkY < chunkColumnSize; chunkY++)
        {
            var lastChunkY = chunkColumnSize - 1;


            // 1.18 and above always contains all chunk section in data
            // 1.17 and 1.17.1 need vertical strip bitmask to know if the chunk section is included
            // Non-air block count inside chunk section, for lighting purposes
            int blockCnt = reader.ReadSignedShort();

            // Read Block states (Type: Paletted Container)
            Chunk? chunk = ReadChunk(ref reader);

            //We have our chunk, save the chunk into the world
            //world.StoreChunk(chunkX, chunkY, chunkZ, chunkColumnSize, chunk, chunkY == lastChunkY);

            // Skip Read Biomes (Type: Paletted Container) - 1.18(1.18.1) and above
            byte bitsPerEntryBiome = reader.ReadUnsignedByte(); // Bits Per Entry
            if (bitsPerEntryBiome == 0)
            {
                reader.ReadVarInt(); // Value
                reader.ReadVarInt(); // Data Array Length
                // Data Array must be empty
            }
            else
            {
                if (bitsPerEntryBiome <= 3)
                {
                    int paletteLength = reader.ReadVarInt(); // Palette Length
                    for (int i = 0; i < paletteLength; i++)
                        reader.ReadVarInt(); // Palette
                }

                int dataArrayLength = reader.ReadVarInt(); // Data Array Length
                reader.Advance(dataArrayLength * 8); // Data Array
            }
        }
    }
}

internal class Block
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
}