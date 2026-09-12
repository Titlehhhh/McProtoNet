using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.update_light", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("ChunkX", "int")]
[PacketField("ChunkZ", "int")]
[PacketField("TrustEdges", "bool", Group = "VUntil754", To = 754)]
[PacketField("SkyLightMaskLegacy", "int", Group = "VUntil754", To = 754)]
[PacketField("BlockLightMaskLegacy", "int", Group = "VUntil754", To = 754)]
[PacketField("EmptySkyLightMaskLegacy", "int", Group = "VUntil754", To = 754)]
[PacketField("EmptyBlockLightMaskLegacy", "int", Group = "VUntil754", To = 754)]
[PacketField("Data", "byte[]", Group = "VUntil754", To = 754)]
[PacketField("TrustEdges", "bool", Group = "V755_762", From = 755, To = 762)]
[PacketField("SkyLightMask", "long[]", Group = "V755_762", From = 755, To = 762)]
[PacketField("BlockLightMask", "long[]", Group = "V755_762", From = 755, To = 762)]
[PacketField("EmptySkyLightMask", "long[]", Group = "V755_762", From = 755, To = 762)]
[PacketField("EmptyBlockLightMask", "long[]", Group = "V755_762", From = 755, To = 762)]
[PacketField("SkyLight", "byte[][]", Group = "V755_762", From = 755, To = 762)]
[PacketField("BlockLight", "byte[][]", Group = "V755_762", From = 755, To = 762)]
[PacketField("SkyLightMask", "long[]", Group = "V763_Last", From = 763)]
[PacketField("BlockLightMask", "long[]", Group = "V763_Last", From = 763)]
[PacketField("EmptySkyLightMask", "long[]", Group = "V763_Last", From = 763)]
[PacketField("EmptyBlockLightMask", "long[]", Group = "V763_Last", From = 763)]
[PacketField("SkyLight", "byte[][]", Group = "V763_Last", From = 763)]
[PacketField("BlockLight", "byte[][]", Group = "V763_Last", From = 763)]
public sealed partial record UpdateLightPacket(int ChunkX, int ChunkZ, UpdateLightPacket.VUntil754Layer? VUntil754 = null, UpdateLightPacket.V755_762Layer? V755_762 = null, UpdateLightPacket.V763_LastLayer? V763_Last = null) : IPacket<UpdateLightPacket>, IPacket
{
    public readonly record struct VUntil754Layer(bool TrustEdges, int SkyLightMaskLegacy, int BlockLightMaskLegacy, int EmptySkyLightMaskLegacy, int EmptyBlockLightMaskLegacy, byte[] Data);
    public readonly record struct V755_762Layer(bool TrustEdges, long[] SkyLightMask, long[] BlockLightMask, long[] EmptySkyLightMask, long[] EmptyBlockLightMask, byte[][] SkyLight, byte[][] BlockLight);
    public readonly record struct V763_LastLayer(long[] SkyLightMask, long[] BlockLightMask, long[] EmptySkyLightMask, long[] EmptyBlockLightMask, byte[][] SkyLight, byte[][] BlockLight);
    public static UpdateLightPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateLightPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var chunkX = reader.ReadVarInt();
            var chunkZ = reader.ReadVarInt();
            var trustEdges = reader.ReadBoolean();
            var skyLightMaskLegacy = reader.ReadVarInt();
            var blockLightMaskLegacy = reader.ReadVarInt();
            var emptySkyLightMaskLegacy = reader.ReadVarInt();
            var emptyBlockLightMaskLegacy = reader.ReadVarInt();
            var data = reader.ReadRestBytes();
            return new UpdateLightPacket(chunkX, chunkZ, VUntil754: new VUntil754Layer(trustEdges, skyLightMaskLegacy, blockLightMaskLegacy, emptySkyLightMaskLegacy, emptyBlockLightMaskLegacy, data));
        }

        if (protocolVersion >= 755 && protocolVersion <= 762)
        {
            var chunkX = reader.ReadVarInt();
            var chunkZ = reader.ReadVarInt();
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
            return new UpdateLightPacket(chunkX, chunkZ, V755_762: new V755_762Layer(trustEdges, skyLightMask, blockLightMask, emptySkyLightMask, emptyBlockLightMask, skyLight, blockLight));
        }

        if (protocolVersion >= 763)
        {
            var chunkX = reader.ReadVarInt();
            var chunkZ = reader.ReadVarInt();
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
            return new UpdateLightPacket(chunkX, chunkZ, V763_Last: new V763_LastLayer(skyLightMask, blockLightMask, emptySkyLightMask, emptyBlockLightMask, skyLight, blockLight));
        }

        throw new System.NotSupportedException($"UpdateLightPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateLightPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var layer = VUntil754 ?? throw new WrongLayerException("UpdateLightPacket", protocolVersion, "VUntil754");
            bool TrustEdges = layer.TrustEdges;
            int SkyLightMaskLegacy = layer.SkyLightMaskLegacy;
            int BlockLightMaskLegacy = layer.BlockLightMaskLegacy;
            int EmptySkyLightMaskLegacy = layer.EmptySkyLightMaskLegacy;
            int EmptyBlockLightMaskLegacy = layer.EmptyBlockLightMaskLegacy;
            byte[] Data = layer.Data;
            writer.WriteVarInt(ChunkX);
            writer.WriteVarInt(ChunkZ);
            writer.WriteBoolean(TrustEdges);
            writer.WriteVarInt(SkyLightMaskLegacy);
            writer.WriteVarInt(BlockLightMaskLegacy);
            writer.WriteVarInt(EmptySkyLightMaskLegacy);
            writer.WriteVarInt(EmptyBlockLightMaskLegacy);
            writer.WriteRestBytes(Data);
            return;
        }

        if (protocolVersion >= 755 && protocolVersion <= 762)
        {
            var layer = V755_762 ?? throw new WrongLayerException("UpdateLightPacket", protocolVersion, "V755_762");
            bool TrustEdges = layer.TrustEdges;
            long[] SkyLightMask = layer.SkyLightMask;
            long[] BlockLightMask = layer.BlockLightMask;
            long[] EmptySkyLightMask = layer.EmptySkyLightMask;
            long[] EmptyBlockLightMask = layer.EmptyBlockLightMask;
            byte[][] SkyLight = layer.SkyLight;
            byte[][] BlockLight = layer.BlockLight;
            writer.WriteVarInt(ChunkX);
            writer.WriteVarInt(ChunkZ);
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

        if (protocolVersion >= 763)
        {
            var layer = V763_Last ?? throw new WrongLayerException("UpdateLightPacket", protocolVersion, "V763_Last");
            long[] SkyLightMask = layer.SkyLightMask;
            long[] BlockLightMask = layer.BlockLightMask;
            long[] EmptySkyLightMask = layer.EmptySkyLightMask;
            long[] EmptyBlockLightMask = layer.EmptyBlockLightMask;
            byte[][] SkyLight = layer.SkyLight;
            byte[][] BlockLight = layer.BlockLight;
            writer.WriteVarInt(ChunkX);
            writer.WriteVarInt(ChunkZ);
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

        throw new System.NotSupportedException($"UpdateLightPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("ChunkX");
        writer.WriteNumberValue(ChunkX);
        writer.WritePropertyName("ChunkZ");
        writer.WriteNumberValue(ChunkZ);
        if (VUntil754 is { } vUntil754)
        {
            writer.WritePropertyName("TrustEdges");
            writer.WriteBooleanValue(vUntil754.TrustEdges);
            writer.WritePropertyName("SkyLightMaskLegacy");
            writer.WriteNumberValue(vUntil754.SkyLightMaskLegacy);
            writer.WritePropertyName("BlockLightMaskLegacy");
            writer.WriteNumberValue(vUntil754.BlockLightMaskLegacy);
            writer.WritePropertyName("EmptySkyLightMaskLegacy");
            writer.WriteNumberValue(vUntil754.EmptySkyLightMaskLegacy);
            writer.WritePropertyName("EmptyBlockLightMaskLegacy");
            writer.WriteNumberValue(vUntil754.EmptyBlockLightMaskLegacy);
            writer.WritePropertyName("Data");
            writer.WriteBase64StringValue(vUntil754.Data);
        }
        else if (V755_762 is { } v755_762)
        {
            writer.WritePropertyName("TrustEdges");
            writer.WriteBooleanValue(v755_762.TrustEdges);
            writer.WritePropertyName("SkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v755_762.SkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v755_762.BlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptySkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v755_762.EmptySkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptyBlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v755_762.EmptyBlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("SkyLight");
            writer.WriteStartArray();
            foreach (var item0 in v755_762.SkyLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLight");
            writer.WriteStartArray();
            foreach (var item0 in v755_762.BlockLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
        }
        else if (V763_Last is { } v763_Last)
        {
            writer.WritePropertyName("SkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v763_Last.SkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v763_Last.BlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptySkyLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v763_Last.EmptySkyLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("EmptyBlockLightMask");
            writer.WriteStartArray();
            foreach (var item0 in v763_Last.EmptyBlockLightMask)
            {
                writer.WriteNumberValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("SkyLight");
            writer.WriteStartArray();
            foreach (var item0 in v763_Last.SkyLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
            writer.WritePropertyName("BlockLight");
            writer.WriteStartArray();
            foreach (var item0 in v763_Last.BlockLight)
            {
                writer.WriteBase64StringValue(item0);
            }

            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.update_light", "UpdateLight", PacketPhase.Play, PacketDirection.Clientbound, 123);

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
