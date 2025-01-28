using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("SteerVehicle", PacketState.Play, PacketDirection.Serverbound)]
    public partial class SteerVehiclePacket : IClientPacket
    {
        public float Sideways { get; set; }
        public float Forward { get; set; }
        public byte Jump { get; set; }

        [PacketSubInfo(340, 767)]
        public sealed partial class V340_767 : SteerVehiclePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Sideways, Forward, Jump);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, float sideways, float forward, byte jump)
            {
                writer.WriteFloat(sideways);
                writer.WriteFloat(forward);
                writer.WriteUnsignedByte(jump);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_767.IsSupportedVersionStatic(protocolVersion))
                V340_767.SerializeInternal(ref writer, protocolVersion, Sideways, Forward, Jump);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.SteerVehicle), protocolVersion);
        }
    }
}