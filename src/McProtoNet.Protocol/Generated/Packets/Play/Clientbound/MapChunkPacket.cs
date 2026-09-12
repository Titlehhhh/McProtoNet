using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.map_chunk", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("X", "int")]
[PacketField("Z", "int")]
[PacketField("ChunkData", "byte[]")]
[PacketField("GroundUp", "int", Group = "VUntil736", To = 736)]
[PacketField("IgnoreOldData", "bool", Group = "VUntil736", To = 736)]
[PacketField("BitMapLegacy", "int", Group = "VUntil736", To = 736)]
[PacketField("HeightmapsLegacy", "NbtTag", Group = "VUntil736", To = 736)]
[PacketField("Biomes", "int[]?", Group = "VUntil736", To = 736)]
[PacketField("BlockEntitiesLegacy", "NbtTag[]", Group = "VUntil736", To = 736)]
[PacketField("GroundUp", "int", Group = "V751_754", From = 751, To = 754)]
[PacketField("BitMapLegacy", "int", Group = "V751_754", From = 751, To = 754)]
[PacketField("HeightmapsLegacy", "NbtTag", Group = "V751_754", From = 751, To = 754)]
[PacketField("Biomes", "int[]?", Group = "V751_754", From = 751, To = 754)]
[PacketField("BlockEntitiesLegacy", "NbtTag[]", Group = "V751_754", From = 751, To = 754)]
[PacketField("BitMap", "long[]", Group = "V755_756", From = 755, To = 756)]
[PacketField("HeightmapsLegacy", "NbtTag", Group = "V755_756", From = 755, To = 756)]
[PacketField("Biomes", "int[]", Group = "V755_756", From = 755, To = 756)]
[PacketField("BlockEntitiesLegacy", "NbtTag[]", Group = "V755_756", From = 755, To = 756)]
[PacketField("HeightmapsLegacy", "NbtTag", Group = "V757_762", From = 757, To = 762)]
[PacketField("BlockEntities", "ChunkBlockEntity[]", Group = "V757_762", From = 757, To = 762)]
[PacketField("TrustEdges", "bool", Group = "V757_762", From = 757, To = 762)]
[PacketField("SkyLightMask", "long[]", Group = "V757_762", From = 757, To = 762)]
[PacketField("BlockLightMask", "long[]", Group = "V757_762", From = 757, To = 762)]
[PacketField("EmptySkyLightMask", "long[]", Group = "V757_762", From = 757, To = 762)]
[PacketField("EmptyBlockLightMask", "long[]", Group = "V757_762", From = 757, To = 762)]
[PacketField("SkyLight", "byte[][]", Group = "V757_762", From = 757, To = 762)]
[PacketField("BlockLight", "byte[][]", Group = "V757_762", From = 757, To = 762)]
[PacketField("HeightmapsLegacy", "NbtTag", Group = "V763", From = 763, To = 763)]
[PacketField("BlockEntities", "ChunkBlockEntity[]", Group = "V763", From = 763, To = 763)]
[PacketField("SkyLightMask", "long[]", Group = "V763", From = 763, To = 763)]
[PacketField("BlockLightMask", "long[]", Group = "V763", From = 763, To = 763)]
[PacketField("EmptySkyLightMask", "long[]", Group = "V763", From = 763, To = 763)]
[PacketField("EmptyBlockLightMask", "long[]", Group = "V763", From = 763, To = 763)]
[PacketField("SkyLight", "byte[][]", Group = "V763", From = 763, To = 763)]
[PacketField("BlockLight", "byte[][]", Group = "V763", From = 763, To = 763)]
[PacketField("HeightmapsLegacy", "NbtTag", Group = "V764_769", From = 764, To = 769)]
[PacketField("BlockEntities", "ChunkBlockEntity[]", Group = "V764_769", From = 764, To = 769)]
[PacketField("SkyLightMask", "long[]", Group = "V764_769", From = 764, To = 769)]
[PacketField("BlockLightMask", "long[]", Group = "V764_769", From = 764, To = 769)]
[PacketField("EmptySkyLightMask", "long[]", Group = "V764_769", From = 764, To = 769)]
[PacketField("EmptyBlockLightMask", "long[]", Group = "V764_769", From = 764, To = 769)]
[PacketField("SkyLight", "byte[][]", Group = "V764_769", From = 764, To = 769)]
[PacketField("BlockLight", "byte[][]", Group = "V764_769", From = 764, To = 769)]
[PacketField("Heightmaps", "Heightmap[]", Group = "V770_Last", From = 770)]
[PacketField("BlockEntities", "ChunkBlockEntity[]", Group = "V770_Last", From = 770)]
[PacketField("SkyLightMask", "long[]", Group = "V770_Last", From = 770)]
[PacketField("BlockLightMask", "long[]", Group = "V770_Last", From = 770)]
[PacketField("EmptySkyLightMask", "long[]", Group = "V770_Last", From = 770)]
[PacketField("EmptyBlockLightMask", "long[]", Group = "V770_Last", From = 770)]
[PacketField("SkyLight", "byte[][]", Group = "V770_Last", From = 770)]
[PacketField("BlockLight", "byte[][]", Group = "V770_Last", From = 770)]
public sealed partial record MapChunkPacket(int X, int Z, byte[] ChunkData, MapChunkPacket.VUntil736Layer? VUntil736 = null, MapChunkPacket.V751_754Layer? V751_754 = null, MapChunkPacket.V755_756Layer? V755_756 = null, MapChunkPacket.V757_762Layer? V757_762 = null, MapChunkPacket.V763Layer? V763 = null, MapChunkPacket.V764_769Layer? V764_769 = null, MapChunkPacket.V770_LastLayer? V770_Last = null) : IPacket<MapChunkPacket>, IPacket
{
    public readonly record struct VUntil736Layer(int GroundUp, bool IgnoreOldData, int BitMapLegacy, NbtTag HeightmapsLegacy, int[]? Biomes, NbtTag[] BlockEntitiesLegacy);
    public readonly record struct V751_754Layer(int GroundUp, int BitMapLegacy, NbtTag HeightmapsLegacy, int[]? Biomes, NbtTag[] BlockEntitiesLegacy);
    public readonly record struct V755_756Layer(long[] BitMap, NbtTag HeightmapsLegacy, int[] Biomes, NbtTag[] BlockEntitiesLegacy);
    public readonly record struct V757_762Layer(NbtTag HeightmapsLegacy, ChunkBlockEntity[] BlockEntities, bool TrustEdges, long[] SkyLightMask, long[] BlockLightMask, long[] EmptySkyLightMask, long[] EmptyBlockLightMask, byte[][] SkyLight, byte[][] BlockLight);
    public readonly record struct V763Layer(NbtTag HeightmapsLegacy, ChunkBlockEntity[] BlockEntities, long[] SkyLightMask, long[] BlockLightMask, long[] EmptySkyLightMask, long[] EmptyBlockLightMask, byte[][] SkyLight, byte[][] BlockLight);
    public readonly record struct V764_769Layer(NbtTag HeightmapsLegacy, ChunkBlockEntity[] BlockEntities, long[] SkyLightMask, long[] BlockLightMask, long[] EmptySkyLightMask, long[] EmptyBlockLightMask, byte[][] SkyLight, byte[][] BlockLight);
    public readonly record struct V770_LastLayer(Heightmap[] Heightmaps, ChunkBlockEntity[] BlockEntities, long[] SkyLightMask, long[] BlockLightMask, long[] EmptySkyLightMask, long[] EmptyBlockLightMask, byte[][] SkyLight, byte[][] BlockLight);
    public static MapChunkPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MapChunkPacket>(protocolVersion);
        if (protocolVersion <= 736)
        {
            var x = reader.ReadSignedInt();
            var z = reader.ReadSignedInt();
            var groundUp = reader.ReadUnsignedByte();
            var ignoreOldData = reader.ReadBoolean();
            var bitMapLegacy = reader.ReadVarInt();
            var heightmapsLegacy = reader.ReadNbtTag(true)!;
            int[]? biomes = default;
            if (groundUp != 0)
            {
                var biomesValue = new int[1024];
                for (int i = 0; i < biomesValue.Length; i++)
                    biomesValue[i] = reader.ReadSignedInt();
                biomes = biomesValue;
            }

            var chunkData = reader.ReadByteArray();
            int blockEntitiesLegacyCount = reader.ReadVarInt();
            var blockEntitiesLegacy = new NbtTag[blockEntitiesLegacyCount];
            for (int i = 0; i < blockEntitiesLegacy.Length; i++)
                blockEntitiesLegacy[i] = reader.ReadNbtTag(true)!;
            return new MapChunkPacket(x, z, chunkData, VUntil736: new VUntil736Layer(groundUp, ignoreOldData, bitMapLegacy, heightmapsLegacy, biomes, blockEntitiesLegacy));
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            var x = reader.ReadSignedInt();
            var z = reader.ReadSignedInt();
            var groundUp = reader.ReadUnsignedByte();
            var bitMapLegacy = reader.ReadVarInt();
            var heightmapsLegacy = reader.ReadNbtTag(true)!;
            int[]? biomes = default;
            if (groundUp != 0)
            {
                int biomesValueCount = reader.ReadVarInt();
                var biomesValue = new int[biomesValueCount];
                for (int i = 0; i < biomesValue.Length; i++)
                    biomesValue[i] = reader.ReadVarInt();
                biomes = biomesValue;
            }

            var chunkData = reader.ReadByteArray();
            int blockEntitiesLegacyCount = reader.ReadVarInt();
            var blockEntitiesLegacy = new NbtTag[blockEntitiesLegacyCount];
            for (int i = 0; i < blockEntitiesLegacy.Length; i++)
                blockEntitiesLegacy[i] = reader.ReadNbtTag(true)!;
            return new MapChunkPacket(x, z, chunkData, V751_754: new V751_754Layer(groundUp, bitMapLegacy, heightmapsLegacy, biomes, blockEntitiesLegacy));
        }

        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            var x = reader.ReadSignedInt();
            var z = reader.ReadSignedInt();
            int bitMapCount = reader.ReadVarInt();
            var bitMap = new long[bitMapCount];
            for (int i = 0; i < bitMap.Length; i++)
                bitMap[i] = reader.ReadSignedLong();
            var heightmapsLegacy = reader.ReadNbtTag(true)!;
            int biomesCount = reader.ReadVarInt();
            var biomes = new int[biomesCount];
            for (int i = 0; i < biomes.Length; i++)
                biomes[i] = reader.ReadVarInt();
            var chunkData = reader.ReadByteArray();
            int blockEntitiesLegacyCount = reader.ReadVarInt();
            var blockEntitiesLegacy = new NbtTag[blockEntitiesLegacyCount];
            for (int i = 0; i < blockEntitiesLegacy.Length; i++)
                blockEntitiesLegacy[i] = reader.ReadNbtTag(true)!;
            return new MapChunkPacket(x, z, chunkData, V755_756: new V755_756Layer(bitMap, heightmapsLegacy, biomes, blockEntitiesLegacy));
        }

        if (protocolVersion >= 757 && protocolVersion <= 762)
        {
            var x = reader.ReadSignedInt();
            var z = reader.ReadSignedInt();
            var heightmapsLegacy = reader.ReadNbtTag(true)!;
            var chunkData = reader.ReadByteArray();
            int blockEntitiesCount = reader.ReadVarInt();
            var blockEntities = new ChunkBlockEntity[blockEntitiesCount];
            for (int i = 0; i < blockEntities.Length; i++)
                blockEntities[i] = reader.ReadType<ChunkBlockEntity>(protocolVersion);
            var trustEdges = reader.ReadBoolean();
            int skyLightMaskCount = reader.ReadVarInt();
            var skyLightMask = new long[skyLightMaskCount];
            for (int i = 0; i < skyLightMask.Length; i++)
                skyLightMask[i] = reader.ReadSignedLong();
            int blockLightMaskCount = reader.ReadVarInt();
            var blockLightMask = new long[blockLightMaskCount];
            for (int i = 0; i < blockLightMask.Length; i++)
                blockLightMask[i] = reader.ReadSignedLong();
            int emptySkyLightMaskCount = reader.ReadVarInt();
            var emptySkyLightMask = new long[emptySkyLightMaskCount];
            for (int i = 0; i < emptySkyLightMask.Length; i++)
                emptySkyLightMask[i] = reader.ReadSignedLong();
            int emptyBlockLightMaskCount = reader.ReadVarInt();
            var emptyBlockLightMask = new long[emptyBlockLightMaskCount];
            for (int i = 0; i < emptyBlockLightMask.Length; i++)
                emptyBlockLightMask[i] = reader.ReadSignedLong();
            int skyLightCount = reader.ReadVarInt();
            var skyLight = new byte[skyLightCount][];
            for (int i = 0; i < skyLight.Length; i++)
                skyLight[i] = reader.ReadByteArray();
            int blockLightCount = reader.ReadVarInt();
            var blockLight = new byte[blockLightCount][];
            for (int i = 0; i < blockLight.Length; i++)
                blockLight[i] = reader.ReadByteArray();
            return new MapChunkPacket(x, z, chunkData, V757_762: new V757_762Layer(heightmapsLegacy, blockEntities, trustEdges, skyLightMask, blockLightMask, emptySkyLightMask, emptyBlockLightMask, skyLight, blockLight));
        }

        if (protocolVersion >= 763 && protocolVersion <= 763)
        {
            var x = reader.ReadSignedInt();
            var z = reader.ReadSignedInt();
            var heightmapsLegacy = reader.ReadNbtTag(true)!;
            var chunkData = reader.ReadByteArray();
            int blockEntitiesCount = reader.ReadVarInt();
            var blockEntities = new ChunkBlockEntity[blockEntitiesCount];
            for (int i = 0; i < blockEntities.Length; i++)
                blockEntities[i] = reader.ReadType<ChunkBlockEntity>(protocolVersion);
            int skyLightMaskCount = reader.ReadVarInt();
            var skyLightMask = new long[skyLightMaskCount];
            for (int i = 0; i < skyLightMask.Length; i++)
                skyLightMask[i] = reader.ReadSignedLong();
            int blockLightMaskCount = reader.ReadVarInt();
            var blockLightMask = new long[blockLightMaskCount];
            for (int i = 0; i < blockLightMask.Length; i++)
                blockLightMask[i] = reader.ReadSignedLong();
            int emptySkyLightMaskCount = reader.ReadVarInt();
            var emptySkyLightMask = new long[emptySkyLightMaskCount];
            for (int i = 0; i < emptySkyLightMask.Length; i++)
                emptySkyLightMask[i] = reader.ReadSignedLong();
            int emptyBlockLightMaskCount = reader.ReadVarInt();
            var emptyBlockLightMask = new long[emptyBlockLightMaskCount];
            for (int i = 0; i < emptyBlockLightMask.Length; i++)
                emptyBlockLightMask[i] = reader.ReadSignedLong();
            int skyLightCount = reader.ReadVarInt();
            var skyLight = new byte[skyLightCount][];
            for (int i = 0; i < skyLight.Length; i++)
                skyLight[i] = reader.ReadByteArray();
            int blockLightCount = reader.ReadVarInt();
            var blockLight = new byte[blockLightCount][];
            for (int i = 0; i < blockLight.Length; i++)
                blockLight[i] = reader.ReadByteArray();
            return new MapChunkPacket(x, z, chunkData, V763: new V763Layer(heightmapsLegacy, blockEntities, skyLightMask, blockLightMask, emptySkyLightMask, emptyBlockLightMask, skyLight, blockLight));
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            var x = reader.ReadSignedInt();
            var z = reader.ReadSignedInt();
            var heightmapsLegacy = reader.ReadNbtTag(false)!;
            var chunkData = reader.ReadByteArray();
            int blockEntitiesCount = reader.ReadVarInt();
            var blockEntities = new ChunkBlockEntity[blockEntitiesCount];
            for (int i = 0; i < blockEntities.Length; i++)
                blockEntities[i] = reader.ReadType<ChunkBlockEntity>(protocolVersion);
            int skyLightMaskCount = reader.ReadVarInt();
            var skyLightMask = new long[skyLightMaskCount];
            for (int i = 0; i < skyLightMask.Length; i++)
                skyLightMask[i] = reader.ReadSignedLong();
            int blockLightMaskCount = reader.ReadVarInt();
            var blockLightMask = new long[blockLightMaskCount];
            for (int i = 0; i < blockLightMask.Length; i++)
                blockLightMask[i] = reader.ReadSignedLong();
            int emptySkyLightMaskCount = reader.ReadVarInt();
            var emptySkyLightMask = new long[emptySkyLightMaskCount];
            for (int i = 0; i < emptySkyLightMask.Length; i++)
                emptySkyLightMask[i] = reader.ReadSignedLong();
            int emptyBlockLightMaskCount = reader.ReadVarInt();
            var emptyBlockLightMask = new long[emptyBlockLightMaskCount];
            for (int i = 0; i < emptyBlockLightMask.Length; i++)
                emptyBlockLightMask[i] = reader.ReadSignedLong();
            int skyLightCount = reader.ReadVarInt();
            var skyLight = new byte[skyLightCount][];
            for (int i = 0; i < skyLight.Length; i++)
                skyLight[i] = reader.ReadByteArray();
            int blockLightCount = reader.ReadVarInt();
            var blockLight = new byte[blockLightCount][];
            for (int i = 0; i < blockLight.Length; i++)
                blockLight[i] = reader.ReadByteArray();
            return new MapChunkPacket(x, z, chunkData, V764_769: new V764_769Layer(heightmapsLegacy, blockEntities, skyLightMask, blockLightMask, emptySkyLightMask, emptyBlockLightMask, skyLight, blockLight));
        }

        if (protocolVersion >= 770)
        {
            var x = reader.ReadSignedInt();
            var z = reader.ReadSignedInt();
            int heightmapsCount = reader.ReadVarInt();
            var heightmaps = new Heightmap[heightmapsCount];
            for (int i = 0; i < heightmaps.Length; i++)
                heightmaps[i] = reader.ReadType<Heightmap>(protocolVersion);
            var chunkData = reader.ReadByteArray();
            int blockEntitiesCount = reader.ReadVarInt();
            var blockEntities = new ChunkBlockEntity[blockEntitiesCount];
            for (int i = 0; i < blockEntities.Length; i++)
                blockEntities[i] = reader.ReadType<ChunkBlockEntity>(protocolVersion);
            int skyLightMaskCount = reader.ReadVarInt();
            var skyLightMask = new long[skyLightMaskCount];
            for (int i = 0; i < skyLightMask.Length; i++)
                skyLightMask[i] = reader.ReadSignedLong();
            int blockLightMaskCount = reader.ReadVarInt();
            var blockLightMask = new long[blockLightMaskCount];
            for (int i = 0; i < blockLightMask.Length; i++)
                blockLightMask[i] = reader.ReadSignedLong();
            int emptySkyLightMaskCount = reader.ReadVarInt();
            var emptySkyLightMask = new long[emptySkyLightMaskCount];
            for (int i = 0; i < emptySkyLightMask.Length; i++)
                emptySkyLightMask[i] = reader.ReadSignedLong();
            int emptyBlockLightMaskCount = reader.ReadVarInt();
            var emptyBlockLightMask = new long[emptyBlockLightMaskCount];
            for (int i = 0; i < emptyBlockLightMask.Length; i++)
                emptyBlockLightMask[i] = reader.ReadSignedLong();
            int skyLightCount = reader.ReadVarInt();
            var skyLight = new byte[skyLightCount][];
            for (int i = 0; i < skyLight.Length; i++)
                skyLight[i] = reader.ReadByteArray();
            int blockLightCount = reader.ReadVarInt();
            var blockLight = new byte[blockLightCount][];
            for (int i = 0; i < blockLight.Length; i++)
                blockLight[i] = reader.ReadByteArray();
            return new MapChunkPacket(x, z, chunkData, V770_Last: new V770_LastLayer(heightmaps, blockEntities, skyLightMask, blockLightMask, emptySkyLightMask, emptyBlockLightMask, skyLight, blockLight));
        }

        throw new System.NotSupportedException($"MapChunkPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MapChunkPacket>(protocolVersion);
        if (protocolVersion <= 736)
        {
            var layer = VUntil736 ?? throw new WrongLayerException("MapChunkPacket", protocolVersion, "VUntil736");
            int GroundUp = layer.GroundUp;
            bool IgnoreOldData = layer.IgnoreOldData;
            int BitMapLegacy = layer.BitMapLegacy;
            NbtTag HeightmapsLegacy = layer.HeightmapsLegacy;
            int[]? Biomes = layer.Biomes;
            NbtTag[] BlockEntitiesLegacy = layer.BlockEntitiesLegacy;
            writer.WriteSignedInt(X);
            writer.WriteSignedInt(Z);
            writer.WriteUnsignedByte((byte)GroundUp);
            writer.WriteBoolean(IgnoreOldData);
            writer.WriteVarInt(BitMapLegacy);
            writer.WriteNbt(HeightmapsLegacy, true);
            if ((byte)GroundUp != 0)
            {
                var biomesValue = Biomes ?? throw new System.InvalidOperationException("Biomes is required at this protocol version.");
                foreach (var biomesItem in biomesValue)
                    writer.WriteSignedInt(biomesItem);
            }

            writer.WriteByteArray(ChunkData);
            writer.WriteVarInt(BlockEntitiesLegacy.Length);
            foreach (var blockEntitiesLegacyItem in BlockEntitiesLegacy)
                writer.WriteNbt(blockEntitiesLegacyItem, true);
            return;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            var layer = V751_754 ?? throw new WrongLayerException("MapChunkPacket", protocolVersion, "V751_754");
            int GroundUp = layer.GroundUp;
            int BitMapLegacy = layer.BitMapLegacy;
            NbtTag HeightmapsLegacy = layer.HeightmapsLegacy;
            int[]? Biomes = layer.Biomes;
            NbtTag[] BlockEntitiesLegacy = layer.BlockEntitiesLegacy;
            writer.WriteSignedInt(X);
            writer.WriteSignedInt(Z);
            writer.WriteUnsignedByte((byte)GroundUp);
            writer.WriteVarInt(BitMapLegacy);
            writer.WriteNbt(HeightmapsLegacy, true);
            if ((byte)GroundUp != 0)
            {
                var biomesValue = Biomes ?? throw new System.InvalidOperationException("Biomes is required at this protocol version.");
                writer.WriteVarInt(biomesValue.Length);
                foreach (var biomesItem in biomesValue)
                    writer.WriteVarInt(biomesItem);
            }

            writer.WriteByteArray(ChunkData);
            writer.WriteVarInt(BlockEntitiesLegacy.Length);
            foreach (var blockEntitiesLegacyItem in BlockEntitiesLegacy)
                writer.WriteNbt(blockEntitiesLegacyItem, true);
            return;
        }

        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            var layer = V755_756 ?? throw new WrongLayerException("MapChunkPacket", protocolVersion, "V755_756");
            long[] BitMap = layer.BitMap;
            NbtTag HeightmapsLegacy = layer.HeightmapsLegacy;
            int[]? Biomes = layer.Biomes;
            NbtTag[] BlockEntitiesLegacy = layer.BlockEntitiesLegacy;
            writer.WriteSignedInt(X);
            writer.WriteSignedInt(Z);
            writer.WriteVarInt(BitMap.Length);
            foreach (var bitMapItem in BitMap)
                writer.WriteSignedLong(bitMapItem);
            writer.WriteNbt(HeightmapsLegacy, true);
            var biomesValue = Biomes ?? throw new System.InvalidOperationException("Biomes is required at this protocol version.");
            writer.WriteVarInt(biomesValue.Length);
            foreach (var biomesItem in biomesValue)
                writer.WriteVarInt(biomesItem);
            writer.WriteByteArray(ChunkData);
            writer.WriteVarInt(BlockEntitiesLegacy.Length);
            foreach (var blockEntitiesLegacyItem in BlockEntitiesLegacy)
                writer.WriteNbt(blockEntitiesLegacyItem, true);
            return;
        }

        if (protocolVersion >= 757 && protocolVersion <= 762)
        {
            var layer = V757_762 ?? throw new WrongLayerException("MapChunkPacket", protocolVersion, "V757_762");
            NbtTag HeightmapsLegacy = layer.HeightmapsLegacy;
            ChunkBlockEntity[] BlockEntities = layer.BlockEntities;
            bool TrustEdges = layer.TrustEdges;
            long[] SkyLightMask = layer.SkyLightMask;
            long[] BlockLightMask = layer.BlockLightMask;
            long[] EmptySkyLightMask = layer.EmptySkyLightMask;
            long[] EmptyBlockLightMask = layer.EmptyBlockLightMask;
            byte[][] SkyLight = layer.SkyLight;
            byte[][] BlockLight = layer.BlockLight;
            writer.WriteSignedInt(X);
            writer.WriteSignedInt(Z);
            writer.WriteNbt(HeightmapsLegacy, true);
            writer.WriteByteArray(ChunkData);
            writer.WriteVarInt(BlockEntities.Length);
            foreach (var blockEntitiesItem in BlockEntities)
                writer.WriteType<ChunkBlockEntity>(blockEntitiesItem, protocolVersion);
            writer.WriteBoolean(TrustEdges);
            writer.WriteVarInt(SkyLightMask.Length);
            foreach (var skyLightMaskItem in SkyLightMask)
                writer.WriteSignedLong(skyLightMaskItem);
            writer.WriteVarInt(BlockLightMask.Length);
            foreach (var blockLightMaskItem in BlockLightMask)
                writer.WriteSignedLong(blockLightMaskItem);
            writer.WriteVarInt(EmptySkyLightMask.Length);
            foreach (var emptySkyLightMaskItem in EmptySkyLightMask)
                writer.WriteSignedLong(emptySkyLightMaskItem);
            writer.WriteVarInt(EmptyBlockLightMask.Length);
            foreach (var emptyBlockLightMaskItem in EmptyBlockLightMask)
                writer.WriteSignedLong(emptyBlockLightMaskItem);
            writer.WriteVarInt(SkyLight.Length);
            foreach (var skyLightItem in SkyLight)
                writer.WriteByteArray(skyLightItem);
            writer.WriteVarInt(BlockLight.Length);
            foreach (var blockLightItem in BlockLight)
                writer.WriteByteArray(blockLightItem);
            return;
        }

        if (protocolVersion >= 763 && protocolVersion <= 763)
        {
            var layer = V763 ?? throw new WrongLayerException("MapChunkPacket", protocolVersion, "V763");
            NbtTag HeightmapsLegacy = layer.HeightmapsLegacy;
            ChunkBlockEntity[] BlockEntities = layer.BlockEntities;
            long[] SkyLightMask = layer.SkyLightMask;
            long[] BlockLightMask = layer.BlockLightMask;
            long[] EmptySkyLightMask = layer.EmptySkyLightMask;
            long[] EmptyBlockLightMask = layer.EmptyBlockLightMask;
            byte[][] SkyLight = layer.SkyLight;
            byte[][] BlockLight = layer.BlockLight;
            writer.WriteSignedInt(X);
            writer.WriteSignedInt(Z);
            writer.WriteNbt(HeightmapsLegacy, true);
            writer.WriteByteArray(ChunkData);
            writer.WriteVarInt(BlockEntities.Length);
            foreach (var blockEntitiesItem in BlockEntities)
                writer.WriteType<ChunkBlockEntity>(blockEntitiesItem, protocolVersion);
            writer.WriteVarInt(SkyLightMask.Length);
            foreach (var skyLightMaskItem in SkyLightMask)
                writer.WriteSignedLong(skyLightMaskItem);
            writer.WriteVarInt(BlockLightMask.Length);
            foreach (var blockLightMaskItem in BlockLightMask)
                writer.WriteSignedLong(blockLightMaskItem);
            writer.WriteVarInt(EmptySkyLightMask.Length);
            foreach (var emptySkyLightMaskItem in EmptySkyLightMask)
                writer.WriteSignedLong(emptySkyLightMaskItem);
            writer.WriteVarInt(EmptyBlockLightMask.Length);
            foreach (var emptyBlockLightMaskItem in EmptyBlockLightMask)
                writer.WriteSignedLong(emptyBlockLightMaskItem);
            writer.WriteVarInt(SkyLight.Length);
            foreach (var skyLightItem in SkyLight)
                writer.WriteByteArray(skyLightItem);
            writer.WriteVarInt(BlockLight.Length);
            foreach (var blockLightItem in BlockLight)
                writer.WriteByteArray(blockLightItem);
            return;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            var layer = V764_769 ?? throw new WrongLayerException("MapChunkPacket", protocolVersion, "V764_769");
            NbtTag HeightmapsLegacy = layer.HeightmapsLegacy;
            ChunkBlockEntity[] BlockEntities = layer.BlockEntities;
            long[] SkyLightMask = layer.SkyLightMask;
            long[] BlockLightMask = layer.BlockLightMask;
            long[] EmptySkyLightMask = layer.EmptySkyLightMask;
            long[] EmptyBlockLightMask = layer.EmptyBlockLightMask;
            byte[][] SkyLight = layer.SkyLight;
            byte[][] BlockLight = layer.BlockLight;
            writer.WriteSignedInt(X);
            writer.WriteSignedInt(Z);
            writer.WriteNbt(HeightmapsLegacy);
            writer.WriteByteArray(ChunkData);
            writer.WriteVarInt(BlockEntities.Length);
            foreach (var blockEntitiesItem in BlockEntities)
                writer.WriteType<ChunkBlockEntity>(blockEntitiesItem, protocolVersion);
            writer.WriteVarInt(SkyLightMask.Length);
            foreach (var skyLightMaskItem in SkyLightMask)
                writer.WriteSignedLong(skyLightMaskItem);
            writer.WriteVarInt(BlockLightMask.Length);
            foreach (var blockLightMaskItem in BlockLightMask)
                writer.WriteSignedLong(blockLightMaskItem);
            writer.WriteVarInt(EmptySkyLightMask.Length);
            foreach (var emptySkyLightMaskItem in EmptySkyLightMask)
                writer.WriteSignedLong(emptySkyLightMaskItem);
            writer.WriteVarInt(EmptyBlockLightMask.Length);
            foreach (var emptyBlockLightMaskItem in EmptyBlockLightMask)
                writer.WriteSignedLong(emptyBlockLightMaskItem);
            writer.WriteVarInt(SkyLight.Length);
            foreach (var skyLightItem in SkyLight)
                writer.WriteByteArray(skyLightItem);
            writer.WriteVarInt(BlockLight.Length);
            foreach (var blockLightItem in BlockLight)
                writer.WriteByteArray(blockLightItem);
            return;
        }

        if (protocolVersion >= 770)
        {
            var layer = V770_Last ?? throw new WrongLayerException("MapChunkPacket", protocolVersion, "V770_Last");
            Heightmap[] Heightmaps = layer.Heightmaps;
            ChunkBlockEntity[] BlockEntities = layer.BlockEntities;
            long[] SkyLightMask = layer.SkyLightMask;
            long[] BlockLightMask = layer.BlockLightMask;
            long[] EmptySkyLightMask = layer.EmptySkyLightMask;
            long[] EmptyBlockLightMask = layer.EmptyBlockLightMask;
            byte[][] SkyLight = layer.SkyLight;
            byte[][] BlockLight = layer.BlockLight;
            writer.WriteSignedInt(X);
            writer.WriteSignedInt(Z);
            writer.WriteVarInt(Heightmaps.Length);
            foreach (var heightmapsItem in Heightmaps)
                writer.WriteType<Heightmap>(heightmapsItem, protocolVersion);
            writer.WriteByteArray(ChunkData);
            writer.WriteVarInt(BlockEntities.Length);
            foreach (var blockEntitiesItem in BlockEntities)
                writer.WriteType<ChunkBlockEntity>(blockEntitiesItem, protocolVersion);
            writer.WriteVarInt(SkyLightMask.Length);
            foreach (var skyLightMaskItem in SkyLightMask)
                writer.WriteSignedLong(skyLightMaskItem);
            writer.WriteVarInt(BlockLightMask.Length);
            foreach (var blockLightMaskItem in BlockLightMask)
                writer.WriteSignedLong(blockLightMaskItem);
            writer.WriteVarInt(EmptySkyLightMask.Length);
            foreach (var emptySkyLightMaskItem in EmptySkyLightMask)
                writer.WriteSignedLong(emptySkyLightMaskItem);
            writer.WriteVarInt(EmptyBlockLightMask.Length);
            foreach (var emptyBlockLightMaskItem in EmptyBlockLightMask)
                writer.WriteSignedLong(emptyBlockLightMaskItem);
            writer.WriteVarInt(SkyLight.Length);
            foreach (var skyLightItem in SkyLight)
                writer.WriteByteArray(skyLightItem);
            writer.WriteVarInt(BlockLight.Length);
            foreach (var blockLightItem in BlockLight)
                writer.WriteByteArray(blockLightItem);
            return;
        }

        throw new System.NotSupportedException($"MapChunkPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("X");
        writer.WriteNumberValue(X);
        writer.WritePropertyName("Z");
        writer.WriteNumberValue(Z);
        writer.WritePropertyName("ChunkData");
        writer.WriteBase64StringValue(ChunkData);
        if (VUntil736 is { } vUntil736)
        {
            writer.WritePropertyName("GroundUp");
            writer.WriteNumberValue(vUntil736.GroundUp);
            writer.WritePropertyName("IgnoreOldData");
            writer.WriteBooleanValue(vUntil736.IgnoreOldData);
            writer.WritePropertyName("BitMapLegacy");
            writer.WriteNumberValue(vUntil736.BitMapLegacy);
            writer.WritePropertyName("HeightmapsLegacy");
            vUntil736.HeightmapsLegacy.WriteJson(writer);
            if (vUntil736.Biomes is { } biomesValue)
            {
                writer.WritePropertyName("Biomes");
                writer.WriteStartArray();
                foreach (var item0 in biomesValue)
                {
                    writer.WriteNumberValue(item0);
                }

                writer.WriteEndArray();
            }

            writer.WritePropertyName("BlockEntitiesLegacy");
            writer.WriteStartArray();
            foreach (var item0 in vUntil736.BlockEntitiesLegacy)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
        }
        else if (V751_754 is { } v751_754)
        {
            writer.WritePropertyName("GroundUp");
            writer.WriteNumberValue(v751_754.GroundUp);
            writer.WritePropertyName("BitMapLegacy");
            writer.WriteNumberValue(v751_754.BitMapLegacy);
            writer.WritePropertyName("HeightmapsLegacy");
            v751_754.HeightmapsLegacy.WriteJson(writer);
            if (v751_754.Biomes is { } biomesValue)
            {
                writer.WritePropertyName("Biomes");
                writer.WriteStartArray();
                foreach (var item0 in biomesValue)
                {
                    writer.WriteNumberValue(item0);
                }

                writer.WriteEndArray();
            }

            writer.WritePropertyName("BlockEntitiesLegacy");
            writer.WriteStartArray();
            foreach (var item0 in v751_754.BlockEntitiesLegacy)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
        }
        else if (V755_756 is { } v755_756)
        {
            writer.WritePropertyName("BitMap");
            writer.WriteStartArray();
            foreach (var item0 in v755_756.BitMap)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("HeightmapsLegacy");
            v755_756.HeightmapsLegacy.WriteJson(writer);
            writer.WritePropertyName("Biomes");
            writer.WriteStartArray();
            foreach (var item0 in v755_756.Biomes)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockEntitiesLegacy");
            writer.WriteStartArray();
            foreach (var item0 in v755_756.BlockEntitiesLegacy)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
        }
        else if (V757_762 is { } v757_762)
        {
            writer.WritePropertyName("HeightmapsLegacy");
            v757_762.HeightmapsLegacy.WriteJson(writer);
            writer.WritePropertyName("BlockEntities");
            writer.WriteStartArray();
            foreach (var item0 in v757_762.BlockEntities)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("TrustEdges");
            writer.WriteBooleanValue(v757_762.TrustEdges);
            writer.WritePropertyName("SkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v757_762.SkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v757_762.BlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptySkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v757_762.EmptySkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptyBlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v757_762.EmptyBlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("SkyLight");
            writer.WriteStartArray();
            foreach (var item0 in v757_762.SkyLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLight");
            writer.WriteStartArray();
            foreach (var item0 in v757_762.BlockLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
        }
        else if (V763 is { } v763)
        {
            writer.WritePropertyName("HeightmapsLegacy");
            v763.HeightmapsLegacy.WriteJson(writer);
            writer.WritePropertyName("BlockEntities");
            writer.WriteStartArray();
            foreach (var item0 in v763.BlockEntities)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("SkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v763.SkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v763.BlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptySkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v763.EmptySkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptyBlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v763.EmptyBlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("SkyLight");
            writer.WriteStartArray();
            foreach (var item0 in v763.SkyLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLight");
            writer.WriteStartArray();
            foreach (var item0 in v763.BlockLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
        }
        else if (V764_769 is { } v764_769)
        {
            writer.WritePropertyName("HeightmapsLegacy");
            v764_769.HeightmapsLegacy.WriteJson(writer);
            writer.WritePropertyName("BlockEntities");
            writer.WriteStartArray();
            foreach (var item0 in v764_769.BlockEntities)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("SkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v764_769.SkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v764_769.BlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptySkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v764_769.EmptySkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptyBlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v764_769.EmptyBlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("SkyLight");
            writer.WriteStartArray();
            foreach (var item0 in v764_769.SkyLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLight");
            writer.WriteStartArray();
            foreach (var item0 in v764_769.BlockLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
        }
        else if (V770_Last is { } v770_Last)
        {
            writer.WritePropertyName("Heightmaps");
            writer.WriteStartArray();
            foreach (var item0 in v770_Last.Heightmaps)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockEntities");
            writer.WriteStartArray();
            foreach (var item0 in v770_Last.BlockEntities)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("SkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v770_Last.SkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v770_Last.BlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptySkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v770_Last.EmptySkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptyBlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v770_Last.EmptyBlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("SkyLight");
            writer.WriteStartArray();
            foreach (var item0 in v770_Last.SkyLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLight");
            writer.WriteStartArray();
            foreach (var item0 in v770_Last.BlockLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.map_chunk", "MapChunk", PacketPhase.Play, PacketDirection.Clientbound, 59);

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
