using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("SteerBoat", PacketState.Play, PacketDirection.Serverbound)]
    public partial class SteerBoatPacket : IClientPacket
    {
        public bool LeftPaddle { get; set; }
        public bool RightPaddle { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : SteerBoatPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, LeftPaddle, RightPaddle);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                bool leftPaddle, bool rightPaddle)
            {
                writer.WriteBoolean(leftPaddle);
                writer.WriteBoolean(rightPaddle);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_769.IsSupportedVersionStatic(protocolVersion))
                V340_769.SerializeInternal(ref writer, protocolVersion, LeftPaddle, RightPaddle);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.SteerBoat), protocolVersion);
        }
    }
}