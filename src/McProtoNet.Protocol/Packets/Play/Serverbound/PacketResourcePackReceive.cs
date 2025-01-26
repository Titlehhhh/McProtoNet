using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class ResourcePackReceivePacket : IClientPacket
    {
        public int Result { get; set; }

        public sealed class V340_764 : ResourcePackReceivePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Result);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int result)
            {
                writer.WriteVarInt(result);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 764;
            }
        }

        public sealed class V765_769 : ResourcePackReceivePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Uuid, Result);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Guid uuid, int result)
            {
                writer.WriteUUID(uuid);
                writer.WriteVarInt(result);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 765 and <= 769;
            }

            public Guid Uuid { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_764.SupportedVersion(protocolVersion) || V765_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_764.SupportedVersion(protocolVersion))
                V340_764.SerializeInternal(ref writer, protocolVersion, Result);
            else if (V765_769.SupportedVersion(protocolVersion))
                V765_769.SerializeInternal(ref writer, protocolVersion, default, Result);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.ResourcePackReceive), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.ResourcePackReceive;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}