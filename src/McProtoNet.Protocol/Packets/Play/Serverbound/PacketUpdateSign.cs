using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class UpdateSignPacket : IClientPacket
    {
        public Position Location { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }

        public sealed class V340_762 : UpdateSignPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Text1, Text2, Text3, Text4);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Position location, string text1, string text2, string text3, string text4)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteString(text1);
                writer.WriteString(text2);
                writer.WriteString(text3);
                writer.WriteString(text4);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 762;
            }
        }

        public sealed class V763_769 : UpdateSignPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, IsFrontText, Text1, Text2, Text3, Text4);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Position location, bool isFrontText, string text1, string text2, string text3, string text4)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteBoolean(isFrontText);
                writer.WriteString(text1);
                writer.WriteString(text2);
                writer.WriteString(text3);
                writer.WriteString(text4);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 763 and <= 769;
            }

            public bool IsFrontText { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_762.SupportedVersion(protocolVersion) || V763_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_762.SupportedVersion(protocolVersion))
                V340_762.SerializeInternal(ref writer, protocolVersion, Location, Text1, Text2, Text3, Text4);
            else if (V763_769.SupportedVersion(protocolVersion))
                V763_769.SerializeInternal(ref writer, protocolVersion, Location, false, Text1, Text2, Text3, Text4);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.UpdateSign), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.UpdateSign;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}