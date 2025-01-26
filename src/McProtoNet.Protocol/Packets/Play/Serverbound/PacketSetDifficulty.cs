using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class SetDifficultyPacket : IClientPacket
    {
        public byte NewDifficulty { get; set; }

        public sealed class V477_769 : SetDifficultyPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, NewDifficulty);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                byte newDifficulty)
            {
                writer.WriteUnsignedByte(newDifficulty);
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
                V477_769.SerializeInternal(ref writer, protocolVersion, NewDifficulty);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.SetDifficulty), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.SetDifficulty;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}