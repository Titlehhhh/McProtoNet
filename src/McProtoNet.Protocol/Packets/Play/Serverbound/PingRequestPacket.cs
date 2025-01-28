using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("PingRequest", PacketState.Play, PacketDirection.Serverbound)]
    public partial class PingRequestPacket : IClientPacket
    {
        public long Id { get; set; }

        [PacketSubInfo(764, 769)]
        public sealed partial class V764_769 : PingRequestPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Id);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, long id)
            {
                writer.WriteSignedLong(id);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V764_769.IsSupportedVersionStatic(protocolVersion))
                V764_769.SerializeInternal(ref writer, protocolVersion, Id);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.PingRequest), protocolVersion);
        }
    }
}