using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("UseItem", PacketState.Play, PacketDirection.Serverbound)]
    public partial class UseItemPacket : IClientPacket
    {
        public int Hand { get; set; }

        [PacketSubInfo(340, 758)]
        public sealed partial class V340_758 : UseItemPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Hand);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int hand)
            {
                writer.WriteVarInt(hand);
            }
        }

        [PacketSubInfo(759, 766)]
        public sealed partial class V759_766 : UseItemPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Hand, Sequence);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int hand, int sequence)
            {
                writer.WriteVarInt(hand);
                writer.WriteVarInt(sequence);
            }

            public int Sequence { get; set; }
        }

        [PacketSubInfo(767, 769)]
        public sealed partial class V767_769 : UseItemPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Hand, Sequence, Rotation);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int hand, int sequence, Vector2 rotation)
            {
                writer.WriteVarInt(hand);
                writer.WriteVarInt(sequence);
                writer.WriteVector2(rotation, protocolVersion);
            }

            public int Sequence { get; set; }
            public Vector2 Rotation { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_758.IsSupportedVersionStatic(protocolVersion))
                V340_758.SerializeInternal(ref writer, protocolVersion, Hand);
            else if (V759_766.IsSupportedVersionStatic(protocolVersion))
                V759_766.SerializeInternal(ref writer, protocolVersion, Hand, 0);
            else if (V767_769.IsSupportedVersionStatic(protocolVersion))
                V767_769.SerializeInternal(ref writer, protocolVersion, Hand, 0, default);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.UseItem), protocolVersion);
        }
    }
}