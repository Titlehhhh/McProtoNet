using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("SetDifficulty", PacketState.Play, PacketDirection.Serverbound)]
    public partial class SetDifficultyPacket : IClientPacket
    {
        public byte NewDifficulty { get; set; }

        [PacketSubInfo(477, 769)]
        public sealed partial class V477_769 : SetDifficultyPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, NewDifficulty);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                byte newDifficulty)
            {
                writer.WriteUnsignedByte(newDifficulty);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V477_769.IsSupportedVersionStatic(protocolVersion))
                V477_769.SerializeInternal(ref writer, protocolVersion, NewDifficulty);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.SetDifficulty), protocolVersion);
        }
    }
}