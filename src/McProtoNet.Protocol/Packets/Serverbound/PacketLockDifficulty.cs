using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class LockDifficultyPacket : IClientPacket
    {
        public bool Locked { get; set; }

        public sealed class V477_769 : LockDifficultyPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Locked);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, bool locked)
            {
                writer.WriteBoolean(locked);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 477 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V477_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V477_769.SupportedVersion(protocolVersion))
                V477_769.SerializeInternal(ref writer, protocolVersion, Locked);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.LockDifficulty), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.LockDifficulty;

        public ClientPacket GetPacketId() => PacketId;
    }
}