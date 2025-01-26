using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class CustomPayloadPacket : IClientPacket
    {
        public string Channel { get; set; }
        public byte[] Data { get; set; }

        internal sealed class V340_769 : CustomPayloadPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Channel, Data);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                string channel, byte[] data)
            {
                writer.WriteString(channel);
                writer.WriteBuffer(data);
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
                V340_769.SerializeInternal(ref writer, protocolVersion, Channel, Data);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.CustomPayload), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.CustomPayload;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}