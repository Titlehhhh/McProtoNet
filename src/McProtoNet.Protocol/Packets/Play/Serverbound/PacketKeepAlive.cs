using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class KeepAlivePacket : IClientPacket
    {
        public long KeepAliveId { get; set; }

        internal sealed class V340_769 : KeepAlivePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, KeepAliveId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, long keepAliveId)
            {
                writer.WriteSignedLong(keepAliveId);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_769.SupportedVersion(protocolVersion))
                V340_769.SerializeInternal(ref writer, protocolVersion, KeepAliveId);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.KeepAlive), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.KeepAlive;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}