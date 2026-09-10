using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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
[PacketField("SkyLight", "int[][]", Group = "V755_762", From = 755, To = 762)]
[PacketField("BlockLight", "int[][]", Group = "V755_762", From = 755, To = 762)]
[PacketField("SkyLightMask", "long[]", Group = "V763_Last", From = 763)]
[PacketField("BlockLightMask", "long[]", Group = "V763_Last", From = 763)]
[PacketField("EmptySkyLightMask", "long[]", Group = "V763_Last", From = 763)]
[PacketField("EmptyBlockLightMask", "long[]", Group = "V763_Last", From = 763)]
[PacketField("SkyLight", "int[][]", Group = "V763_Last", From = 763)]
[PacketField("BlockLight", "int[][]", Group = "V763_Last", From = 763)]
public sealed partial record UpdateLightPacket(int ChunkX, int ChunkZ, UpdateLightPacket.VUntil754Layer? VUntil754 = null, UpdateLightPacket.V755_762Layer? V755_762 = null, UpdateLightPacket.V763_LastLayer? V763_Last = null) : IPacket<UpdateLightPacket>, IPacket
{
    public readonly record struct VUntil754Layer(bool TrustEdges, int SkyLightMaskLegacy, int BlockLightMaskLegacy, int EmptySkyLightMaskLegacy, int EmptyBlockLightMaskLegacy, byte[] Data);
    public readonly record struct V755_762Layer(bool TrustEdges, long[] SkyLightMask, long[] BlockLightMask, long[] EmptySkyLightMask, long[] EmptyBlockLightMask, int[][] SkyLight, int[][] BlockLight);
    public readonly record struct V763_LastLayer(long[] SkyLightMask, long[] BlockLightMask, long[] EmptySkyLightMask, long[] EmptyBlockLightMask, int[][] SkyLight, int[][] BlockLight);
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
            // TODO(codegen): read 'SkyLight' (Array Array (U8, VarIntCount))
            // TODO(codegen): read 'BlockLight' (Array Array (U8, VarIntCount))
            throw new System.NotImplementedException("TODO(codegen): UpdateLightPacket wire layout is not fully generated for this protocol version.");
        }

        if (protocolVersion >= 763)
        {
            // TODO(codegen): read 'SkyLight' (Array Array (U8, VarIntCount))
            // TODO(codegen): read 'BlockLight' (Array Array (U8, VarIntCount))
            throw new System.NotImplementedException("TODO(codegen): UpdateLightPacket wire layout is not fully generated for this protocol version.");
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
            // TODO(codegen): write 'SkyLight' (Array Array (U8, VarIntCount))
            // TODO(codegen): write 'BlockLight' (Array Array (U8, VarIntCount))
            throw new System.NotImplementedException("TODO(codegen): UpdateLightPacket wire layout is not fully generated for this protocol version.");
        }

        if (protocolVersion >= 763)
        {
            // TODO(codegen): write 'SkyLight' (Array Array (U8, VarIntCount))
            // TODO(codegen): write 'BlockLight' (Array Array (U8, VarIntCount))
            throw new System.NotImplementedException("TODO(codegen): UpdateLightPacket wire layout is not fully generated for this protocol version.");
        }

        throw new System.NotSupportedException($"UpdateLightPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.update_light", "UpdateLight", PacketPhase.Play, PacketDirection.Clientbound, 122);

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
