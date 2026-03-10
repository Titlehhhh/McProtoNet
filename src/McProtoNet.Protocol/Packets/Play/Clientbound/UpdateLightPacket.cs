using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("UpdateLight", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class UpdateLightPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
        new(755, 762),
        new(763, MinecraftVersion.LatestProtocol),
    };

    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }

    public VFirst_754Fields? VFirst_754 { get; set; }
    public V755_762Fields? V755_762 { get; set; }
    public V763_LastFields? V763_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                var fields = VFirst_754 ?? throw new InvalidOperationException("UpdateLight VFirst_754 missing.");
                writer.WriteVarInt(ChunkX);
                writer.WriteVarInt(ChunkZ);
                writer.WriteBoolean(fields.TrustEdges);
                writer.WriteVarInt(fields.SkyLightMask);
                writer.WriteVarInt(fields.BlockLightMask);
                writer.WriteVarInt(fields.EmptySkyLightMask);
                writer.WriteVarInt(fields.EmptyBlockLightMask);
                writer.WriteBuffer(fields.Data);
                return;
            }
            case >= 755 and <= 762:
            {
                var fields = V755_762 ?? throw new InvalidOperationException("UpdateLight V755_762 missing.");
                writer.WriteVarInt(ChunkX);
                writer.WriteVarInt(ChunkZ);
                writer.WriteBoolean(fields.TrustEdges);
                writer.WriteVarInt(fields.SkyLightMask.Length);
                for (int i = 0; i < fields.SkyLightMask.Length; i++) writer.WriteSignedLong(fields.SkyLightMask[i]);
                writer.WriteVarInt(fields.BlockLightMask.Length);
                for (int i = 0; i < fields.BlockLightMask.Length; i++) writer.WriteSignedLong(fields.BlockLightMask[i]);
                writer.WriteVarInt(fields.EmptySkyLightMask.Length);
                for (int i = 0; i < fields.EmptySkyLightMask.Length; i++) writer.WriteSignedLong(fields.EmptySkyLightMask[i]);
                writer.WriteVarInt(fields.EmptyBlockLightMask.Length);
                for (int i = 0; i < fields.EmptyBlockLightMask.Length; i++) writer.WriteSignedLong(fields.EmptyBlockLightMask[i]);
                writer.WriteVarInt(fields.SkyLight.Length);
                for (int i = 0; i < fields.SkyLight.Length; i++)
                {
                    writer.WriteVarInt(fields.SkyLight[i].Length);
                    writer.WriteBuffer(fields.SkyLight[i]);
                }
                writer.WriteVarInt(fields.BlockLight.Length);
                for (int i = 0; i < fields.BlockLight.Length; i++)
                {
                    writer.WriteVarInt(fields.BlockLight[i].Length);
                    writer.WriteBuffer(fields.BlockLight[i]);
                }
                return;
            }
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V763_Last ?? throw new InvalidOperationException("UpdateLight V763_Last missing.");
                writer.WriteVarInt(ChunkX);
                writer.WriteVarInt(ChunkZ);
                writer.WriteVarInt(fields.SkyLightMask.Length);
                for (int i = 0; i < fields.SkyLightMask.Length; i++) writer.WriteSignedLong(fields.SkyLightMask[i]);
                writer.WriteVarInt(fields.BlockLightMask.Length);
                for (int i = 0; i < fields.BlockLightMask.Length; i++) writer.WriteSignedLong(fields.BlockLightMask[i]);
                writer.WriteVarInt(fields.EmptySkyLightMask.Length);
                for (int i = 0; i < fields.EmptySkyLightMask.Length; i++) writer.WriteSignedLong(fields.EmptySkyLightMask[i]);
                writer.WriteVarInt(fields.EmptyBlockLightMask.Length);
                for (int i = 0; i < fields.EmptyBlockLightMask.Length; i++) writer.WriteSignedLong(fields.EmptyBlockLightMask[i]);
                writer.WriteVarInt(fields.SkyLight.Length);
                for (int i = 0; i < fields.SkyLight.Length; i++)
                {
                    writer.WriteVarInt(fields.SkyLight[i].Length);
                    writer.WriteBuffer(fields.SkyLight[i]);
                }
                writer.WriteVarInt(fields.BlockLight.Length);
                for (int i = 0; i < fields.BlockLight.Length; i++)
                {
                    writer.WriteVarInt(fields.BlockLight[i].Length);
                    writer.WriteBuffer(fields.BlockLight[i]);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.UpdateLight), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                ChunkX = reader.ReadVarInt();
                ChunkZ = reader.ReadVarInt();
                VFirst_754 = new VFirst_754Fields
                {
                    TrustEdges = reader.ReadBoolean(),
                    SkyLightMask = reader.ReadVarInt(),
                    BlockLightMask = reader.ReadVarInt(),
                    EmptySkyLightMask = reader.ReadVarInt(),
                    EmptyBlockLightMask = reader.ReadVarInt(),
                    Data = reader.ReadRestBuffer()
                };
                return;
            case >= 755 and <= 762:
            {
                ChunkX = reader.ReadVarInt();
                ChunkZ = reader.ReadVarInt();
                var fields = new V755_762Fields
                {
                    TrustEdges = reader.ReadBoolean()
                };

                int skyMaskCount = reader.ReadVarInt();
                fields.SkyLightMask = skyMaskCount == 0 ? Array.Empty<long>() : new long[skyMaskCount];
                for (int i = 0; i < fields.SkyLightMask.Length; i++) fields.SkyLightMask[i] = reader.ReadSignedLong();

                int blockMaskCount = reader.ReadVarInt();
                fields.BlockLightMask = blockMaskCount == 0 ? Array.Empty<long>() : new long[blockMaskCount];
                for (int i = 0; i < fields.BlockLightMask.Length; i++) fields.BlockLightMask[i] = reader.ReadSignedLong();

                int emptySkyCount = reader.ReadVarInt();
                fields.EmptySkyLightMask = emptySkyCount == 0 ? Array.Empty<long>() : new long[emptySkyCount];
                for (int i = 0; i < fields.EmptySkyLightMask.Length; i++) fields.EmptySkyLightMask[i] = reader.ReadSignedLong();

                int emptyBlockCount = reader.ReadVarInt();
                fields.EmptyBlockLightMask = emptyBlockCount == 0 ? Array.Empty<long>() : new long[emptyBlockCount];
                for (int i = 0; i < fields.EmptyBlockLightMask.Length; i++) fields.EmptyBlockLightMask[i] = reader.ReadSignedLong();

                int skyLightCount = reader.ReadVarInt();
                fields.SkyLight = skyLightCount == 0 ? Array.Empty<byte[]>() : new byte[skyLightCount][];
                for (int i = 0; i < fields.SkyLight.Length; i++)
                {
                    int length = reader.ReadVarInt();
                    fields.SkyLight[i] = reader.ReadBuffer(length);
                }

                int blockLightCount = reader.ReadVarInt();
                fields.BlockLight = blockLightCount == 0 ? Array.Empty<byte[]>() : new byte[blockLightCount][];
                for (int i = 0; i < fields.BlockLight.Length; i++)
                {
                    int length = reader.ReadVarInt();
                    fields.BlockLight[i] = reader.ReadBuffer(length);
                }

                V755_762 = fields;
                return;
            }
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                ChunkX = reader.ReadVarInt();
                ChunkZ = reader.ReadVarInt();
                var fields = new V763_LastFields();

                int skyMaskCount = reader.ReadVarInt();
                fields.SkyLightMask = skyMaskCount == 0 ? Array.Empty<long>() : new long[skyMaskCount];
                for (int i = 0; i < fields.SkyLightMask.Length; i++) fields.SkyLightMask[i] = reader.ReadSignedLong();

                int blockMaskCount = reader.ReadVarInt();
                fields.BlockLightMask = blockMaskCount == 0 ? Array.Empty<long>() : new long[blockMaskCount];
                for (int i = 0; i < fields.BlockLightMask.Length; i++) fields.BlockLightMask[i] = reader.ReadSignedLong();

                int emptySkyCount = reader.ReadVarInt();
                fields.EmptySkyLightMask = emptySkyCount == 0 ? Array.Empty<long>() : new long[emptySkyCount];
                for (int i = 0; i < fields.EmptySkyLightMask.Length; i++) fields.EmptySkyLightMask[i] = reader.ReadSignedLong();

                int emptyBlockCount = reader.ReadVarInt();
                fields.EmptyBlockLightMask = emptyBlockCount == 0 ? Array.Empty<long>() : new long[emptyBlockCount];
                for (int i = 0; i < fields.EmptyBlockLightMask.Length; i++) fields.EmptyBlockLightMask[i] = reader.ReadSignedLong();

                int skyLightCount = reader.ReadVarInt();
                fields.SkyLight = skyLightCount == 0 ? Array.Empty<byte[]>() : new byte[skyLightCount][];
                for (int i = 0; i < fields.SkyLight.Length; i++)
                {
                    int length = reader.ReadVarInt();
                    fields.SkyLight[i] = reader.ReadBuffer(length);
                }

                int blockLightCount = reader.ReadVarInt();
                fields.BlockLight = blockLightCount == 0 ? Array.Empty<byte[]>() : new byte[blockLightCount][];
                for (int i = 0; i < fields.BlockLight.Length; i++)
                {
                    int length = reader.ReadVarInt();
                    fields.BlockLight[i] = reader.ReadBuffer(length);
                }

                V763_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.UpdateLight), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_754Fields
    {
        public bool TrustEdges { get; set; }
        public int SkyLightMask { get; set; }
        public int BlockLightMask { get; set; }
        public int EmptySkyLightMask { get; set; }
        public int EmptyBlockLightMask { get; set; }
        public byte[] Data { get; set; }
    }

    public struct V755_762Fields
    {
        public bool TrustEdges { get; set; }
        public long[] SkyLightMask { get; set; }
        public long[] BlockLightMask { get; set; }
        public long[] EmptySkyLightMask { get; set; }
        public long[] EmptyBlockLightMask { get; set; }
        public byte[][] SkyLight { get; set; }
        public byte[][] BlockLight { get; set; }
    }

    public struct V763_LastFields
    {
        public long[] SkyLightMask { get; set; }
        public long[] BlockLightMask { get; set; }
        public long[] EmptySkyLightMask { get; set; }
        public long[] EmptyBlockLightMask { get; set; }
        public byte[][] SkyLight { get; set; }
        public byte[][] BlockLight { get; set; }
    }
}
