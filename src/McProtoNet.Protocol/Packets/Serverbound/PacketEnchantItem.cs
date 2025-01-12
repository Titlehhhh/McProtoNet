using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class EnchantItemPacket : IClientPacket
    {
        public sbyte Enchantment { get; set; }

        public sealed class V340_767 : EnchantItemPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId, Enchantment);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, sbyte windowId, sbyte enchantment)
            {
                writer.WriteSignedByte(windowId);
                writer.WriteSignedByte(enchantment);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 767;
            }

            public sbyte WindowId { get; set; }
        }

        public sealed class V768_769 : EnchantItemPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId, Enchantment);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int windowId, sbyte enchantment)
            {
                writer.WriteVarInt(windowId);
                writer.WriteSignedByte(enchantment);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }

            public int WindowId { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_767.SupportedVersion(protocolVersion) || V768_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_767.SupportedVersion(protocolVersion))
                V340_767.SerializeInternal(ref writer, protocolVersion, 0, Enchantment);
            else if (V768_769.SupportedVersion(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, default, Enchantment);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.EnchantItem), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.EnchantItem;

        public ClientPacket GetPacketId() => PacketId;
    }
}