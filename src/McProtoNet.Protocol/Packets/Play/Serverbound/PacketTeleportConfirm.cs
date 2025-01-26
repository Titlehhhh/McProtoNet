using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class TeleportConfirmPacket : IClientPacket
    {
        public int TeleportId { get; set; }

        internal sealed class V340_769 : TeleportConfirmPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, TeleportId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int teleportId)
            {
                writer.WriteVarInt(teleportId);
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
                V340_769.SerializeInternal(ref writer, protocolVersion, TeleportId);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.TeleportConfirm), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.TeleportConfirm;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}