using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class UpdateCommandBlockPacket : IClientPacket
    {
        public Position Location { get; set; }
        public string Command { get; set; }
        public int Mode { get; set; }
        public byte Flags { get; set; }

        public sealed class V393_769 : UpdateCommandBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Command, Mode, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Position location, string command, int mode, byte flags)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteString(command);
                writer.WriteVarInt(mode);
                writer.WriteUnsignedByte(flags);
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
                V393_769.SerializeInternal(ref writer, protocolVersion, Location, Command, Mode, Flags);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.UpdateCommandBlock), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.UpdateCommandBlock;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}