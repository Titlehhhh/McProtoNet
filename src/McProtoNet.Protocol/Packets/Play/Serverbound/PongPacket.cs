using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("Pong", PacketState.Play, PacketDirection.Serverbound)]
    public partial class PongPacket : IClientPacket
    {
        public int Id { get; set; }

        [PacketSubInfo(755, 769)]
        public sealed partial class V755_769 : PongPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Id);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int id)
            {
                writer.WriteSignedInt(id);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V755_769.IsSupportedVersionStatic(protocolVersion))
                V755_769.SerializeInternal(ref writer, protocolVersion, Id);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.Pong), protocolVersion);
        }
    }
}