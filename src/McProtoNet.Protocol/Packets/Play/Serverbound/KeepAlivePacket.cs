using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("KeepAlive", PacketState.Play, PacketDirection.Serverbound)]
    public partial class KeepAlivePacket : IClientPacket
    {
        public long KeepAliveId { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : KeepAlivePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, KeepAliveId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                long keepAliveId)
            {
                writer.WriteSignedLong(keepAliveId);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_769.IsSupportedVersionStatic(protocolVersion))
                V340_769.SerializeInternal(ref writer, protocolVersion, KeepAliveId);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.KeepAlive), protocolVersion);
        }
    }
}