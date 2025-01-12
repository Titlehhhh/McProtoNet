using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class PingRequestPacket : IClientPacket
    {
        public long Id { get; set; }

        internal sealed class V764_769 : PingRequestPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Id);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, long id)
            {
                writer.WriteSignedLong(id);
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
                V764_769.SerializeInternal(ref writer, protocolVersion, Id);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.PingRequest), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.PingRequest;

        public ClientPacket GetPacketId() => PacketId;
    }
}