using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class SpectatePacket : IClientPacket
    {
        public Guid Target { get; set; }

        internal sealed class V340_769 : SpectatePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Target);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                Guid target)
            {
                writer.WriteUUID(target);
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
                V340_769.SerializeInternal(ref writer, protocolVersion, Target);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.Spectate), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.Spectate;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}