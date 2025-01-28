using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("ChunkBatchReceived", PacketState.Play, PacketDirection.Serverbound)]
    public partial class ChunkBatchReceivedPacket : IClientPacket
    {
        public float ChunksPerTick { get; set; }

        [PacketSubInfo(764, 769)]
        public sealed partial class V764_769 : ChunkBatchReceivedPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, ChunksPerTick);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, float chunksPerTick)
            {
                writer.WriteFloat(chunksPerTick);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V764_769.IsSupportedVersionStatic(protocolVersion))
                V764_769.SerializeInternal(ref writer, protocolVersion, ChunksPerTick);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.ChunkBatchReceived), protocolVersion);
        }
    }
}