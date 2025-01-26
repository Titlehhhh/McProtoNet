using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class UpdateCommandBlockMinecartPacket : IClientPacket
    {
        public int EntityId { get; set; }
        public string Command { get; set; }
        public bool TrackOutput { get; set; }

        public sealed class V393_769 : UpdateCommandBlockMinecartPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, EntityId, Command, TrackOutput);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int entityId, string command, bool trackOutput)
            {
                writer.WriteVarInt(entityId);
                writer.WriteString(command);
                writer.WriteBoolean(trackOutput);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 393 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V393_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_769.SupportedVersion(protocolVersion))
                V393_769.SerializeInternal(ref writer, protocolVersion, EntityId, Command, TrackOutput);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.UpdateCommandBlockMinecart),
                    protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.UpdateCommandBlockMinecart;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}