using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("EnchantItem", PacketState.Play, PacketDirection.Serverbound)]
    public partial class EnchantItemPacket : IClientPacket
    {
        public sbyte Enchantment { get; set; }

        [PacketSubInfo(340, 767)]
        public sealed partial class V340_767 : EnchantItemPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId, Enchantment);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                sbyte windowId, sbyte enchantment)
            {
                writer.WriteSignedByte(windowId);
                writer.WriteSignedByte(enchantment);
            }

            public sbyte WindowId { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : EnchantItemPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId, Enchantment);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int windowId, sbyte enchantment)
            {
                writer.WriteVarInt(windowId);
                writer.WriteSignedByte(enchantment);
            }

            public int WindowId { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_767.IsSupportedVersionStatic(protocolVersion))
                V340_767.SerializeInternal(ref writer, protocolVersion, 0, Enchantment);
            else if (V768_769.IsSupportedVersionStatic(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, default, Enchantment);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.EnchantItem), protocolVersion);
        }
    }
}