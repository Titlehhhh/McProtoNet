using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class PickItemFromBlockPacket : IClientPacket
    {
        public Position Position { get; set; }
        public bool IncludeData { get; set; }

        public sealed class V769 : PickItemFromBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Position, IncludeData);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                Position position, bool includeData)
            {
                writer.WritePosition(position, protocolVersion);
                writer.WriteBoolean(includeData);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion == 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V769.SupportedVersion(protocolVersion))
                V769.SerializeInternal(ref writer, protocolVersion, Position, IncludeData);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.PickItemFromBlock), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.PickItemFromBlock;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}