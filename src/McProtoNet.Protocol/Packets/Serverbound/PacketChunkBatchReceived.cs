using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class ChunkBatchReceivedPacket : IClientPacket
    {
        public float ChunksPerTick { get; set; }

        internal sealed class V764_769 : ChunkBatchReceivedPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, ChunksPerTick);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, float chunksPerTick)
            {
                writer.WriteFloat(chunksPerTick);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 764 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V764_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V764_769.SupportedVersion(protocolVersion))
                V764_769.SerializeInternal(ref writer, protocolVersion, ChunksPerTick);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.ChunkBatchReceived), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.ChunkBatchReceived;

        public ClientPacket GetPacketId() => PacketId;
    }
}