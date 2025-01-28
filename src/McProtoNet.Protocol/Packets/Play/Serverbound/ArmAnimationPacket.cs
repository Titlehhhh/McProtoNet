using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("ArmAnimation", PacketState.Play, PacketDirection.Serverbound)]
    public partial class ArmAnimationPacket : IClientPacket
    {
        public int Hand { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : ArmAnimationPacket
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

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_769.IsSupportedVersionStatic(protocolVersion))
                V340_769.SerializeInternal(ref writer, protocolVersion, Hand);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.ArmAnimation), protocolVersion);
        }
    }
}