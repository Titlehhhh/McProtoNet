using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("MultiBlockChange", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class MultiBlockChangePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 736),
        new(751, 756),
        new(757, 758),
        new(759, 759),
        new(760, 762),
        new(763, MinecraftVersion.LatestProtocol),
    };

    public VFirst_736Fields? VFirst_736 { get; set; }
    public V751_756Fields? V751_756 { get; set; }
    public V757_758Fields? V757_758 { get; set; }
    public V759Fields? V759 { get; set; }
    public V760_762Fields? V760_762 { get; set; }
    public V763_LastFields? V763_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 736:
            {
                var fields = VFirst_736 ?? throw new InvalidOperationException("MultiBlockChange VFirst_736 fields missing.");
                writer.WriteSignedInt(fields.ChunkX);
                writer.WriteSignedInt(fields.ChunkZ);
                writer.WriteVarInt(fields.Records.Length);
                for (int i = 0; i < fields.Records.Length; i++)
                {
                    writer.WriteUnsignedByte(fields.Records[i].HorizontalPos);
                    writer.WriteUnsignedByte(fields.Records[i].Y);
                    writer.WriteVarInt(fields.Records[i].BlockId);
                }
                return;
            }
            case >= 751 and <= 756:
            {
                var fields = V751_756 ?? throw new InvalidOperationException("MultiBlockChange V751_756 fields missing.");
                WriteChunkCoordinates(writer, fields.ChunkCoordinates);
                writer.WriteBoolean(fields.NotTrustEdges);
                writer.WriteVarInt(fields.Records.Length);
                for (int i = 0; i < fields.Records.Length; i++)
                {
                    writer.WriteVarLong(fields.Records[i]);
                }
                return;
            }
            case >= 757 and <= 758:
            {
                var fields = V757_758 ?? throw new InvalidOperationException("MultiBlockChange V757_758 fields missing.");
                WriteChunkCoordinates(writer, fields.ChunkCoordinates);
                writer.WriteBoolean(fields.NotTrustEdges);
                writer.WriteVarInt(fields.Records.Length);
                for (int i = 0; i < fields.Records.Length; i++)
                {
                    writer.WriteVarLong(fields.Records[i]);
                }
                return;
            }
            case 759:
            {
                var fields = V759 ?? throw new InvalidOperationException("MultiBlockChange V759 fields missing.");
                WriteChunkCoordinates(writer, fields.ChunkCoordinates);
                writer.WriteBoolean(fields.NotTrustEdges);
                writer.WriteVarInt(fields.Records.Length);
                for (int i = 0; i < fields.Records.Length; i++)
                {
                    writer.WriteVarInt(fields.Records[i]);
                }
                return;
            }
            case >= 760 and <= 762:
            {
                var fields = V760_762 ?? throw new InvalidOperationException("MultiBlockChange V760_762 fields missing.");
                WriteChunkCoordinates(writer, fields.ChunkCoordinates);
                writer.WriteBoolean(fields.SuppressLightUpdates);
                writer.WriteVarInt(fields.Records.Length);
                for (int i = 0; i < fields.Records.Length; i++)
                {
                    writer.WriteVarInt(fields.Records[i]);
                }
                return;
            }
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V763_Last ?? throw new InvalidOperationException("MultiBlockChange V763_Last fields missing.");
                WriteChunkCoordinates(writer, fields.ChunkCoordinates);
                writer.WriteVarInt(fields.Records.Length);
                for (int i = 0; i < fields.Records.Length; i++)
                {
                    writer.WriteVarInt(fields.Records[i]);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.MultiBlockChange), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 736:
            {
                VFirst_736 = new VFirst_736Fields
                {
                    ChunkX = reader.ReadSignedInt(),
                    ChunkZ = reader.ReadSignedInt(),
                    Records = ReadLegacyRecords(ref reader)
                };
                return;
            }
            case >= 751 and <= 756:
                V751_756 = new V751_756Fields
                {
                    ChunkCoordinates = ReadChunkCoordinates(ref reader, false),
                    NotTrustEdges = reader.ReadBoolean(),
                    Records = ReadVarLongRecords(ref reader)
                };
                return;
            case >= 757 and <= 758:
                V757_758 = new V757_758Fields
                {
                    ChunkCoordinates = ReadChunkCoordinates(ref reader, true),
                    NotTrustEdges = reader.ReadBoolean(),
                    Records = ReadVarLongRecords(ref reader)
                };
                return;
            case 759:
                V759 = new V759Fields
                {
                    ChunkCoordinates = ReadChunkCoordinates(ref reader, true),
                    NotTrustEdges = reader.ReadBoolean(),
                    Records = ReadVarIntRecords(ref reader)
                };
                return;
            case >= 760 and <= 762:
                V760_762 = new V760_762Fields
                {
                    ChunkCoordinates = ReadChunkCoordinates(ref reader, true),
                    SuppressLightUpdates = reader.ReadBoolean(),
                    Records = ReadVarIntRecords(ref reader)
                };
                return;
            case >= 763 and <= MinecraftVersion.LatestProtocol:
                V763_Last = new V763_LastFields
                {
                    ChunkCoordinates = ReadChunkCoordinates(ref reader, true),
                    Records = ReadVarIntRecords(ref reader)
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.MultiBlockChange), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static LegacyRecord[] ReadLegacyRecords(ref MinecraftPrimitiveReader reader)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<LegacyRecord>();
        }

        var records = new LegacyRecord[count];
        for (int i = 0; i < records.Length; i++)
        {
            records[i] = new LegacyRecord
            {
                HorizontalPos = reader.ReadUnsignedByte(),
                Y = reader.ReadUnsignedByte(),
                BlockId = reader.ReadVarInt()
            };
        }
        return records;
    }

    private static long[] ReadVarLongRecords(ref MinecraftPrimitiveReader reader)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<long>();
        }

        var records = new long[count];
        for (int i = 0; i < records.Length; i++)
        {
            records[i] = reader.ReadVarLong();
        }
        return records;
    }

    private static int[] ReadVarIntRecords(ref MinecraftPrimitiveReader reader)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<int>();
        }

        var records = new int[count];
        for (int i = 0; i < records.Length; i++)
        {
            records[i] = reader.ReadVarInt();
        }
        return records;
    }

    private static ChunkCoordinates ReadChunkCoordinates(ref MinecraftPrimitiveReader reader, bool signedY)
    {
        ulong value = reader.ReadUnsignedLong();
        int x = SignExtend((int)((value >> 42) & 0x3FFFFF), 22);
        int z = SignExtend((int)((value >> 20) & 0x3FFFFF), 22);
        int y = (int)(value & 0xFFFFF);
        if (signedY)
        {
            y = SignExtend(y, 20);
        }
        return new ChunkCoordinates(x, z, y);
    }

    private static void WriteChunkCoordinates(MinecraftPrimitiveWriter writer, ChunkCoordinates coords)
    {
        ulong x = (uint)coords.X & 0x3FFFFF;
        ulong z = (uint)coords.Z & 0x3FFFFF;
        ulong y = (uint)coords.Y & 0xFFFFF;
        ulong packed = (x << 42) | (z << 20) | y;
        writer.WriteUnsignedLong(packed);
    }

    private static int SignExtend(int value, int bits)
    {
        int shift = 32 - bits;
        return (value << shift) >> shift;
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_736Fields
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }
        public LegacyRecord[] Records { get; set; }
    }

    public struct V751_756Fields
    {
        public ChunkCoordinates ChunkCoordinates { get; set; }
        public bool NotTrustEdges { get; set; }
        public long[] Records { get; set; }
    }

    public struct V757_758Fields
    {
        public ChunkCoordinates ChunkCoordinates { get; set; }
        public bool NotTrustEdges { get; set; }
        public long[] Records { get; set; }
    }

    public struct V759Fields
    {
        public ChunkCoordinates ChunkCoordinates { get; set; }
        public bool NotTrustEdges { get; set; }
        public int[] Records { get; set; }
    }

    public struct V760_762Fields
    {
        public ChunkCoordinates ChunkCoordinates { get; set; }
        public bool SuppressLightUpdates { get; set; }
        public int[] Records { get; set; }
    }

    public struct V763_LastFields
    {
        public ChunkCoordinates ChunkCoordinates { get; set; }
        public int[] Records { get; set; }
    }

    public struct ChunkCoordinates
    {
        public int X { get; set; }
        public int Z { get; set; }
        public int Y { get; set; }
    }

    public struct LegacyRecord
    {
        public byte HorizontalPos { get; set; }
        public byte Y { get; set; }
        public int BlockId { get; set; }
    }
}
