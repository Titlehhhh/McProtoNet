using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("ConfigurationAcknowledged", PacketState.Play, PacketDirection.Serverbound)]
    public partial class ConfigurationAcknowledgedPacket : IClientPacket
    {
        [PacketSubInfo(764, 769)]
        public sealed partial class V764_769 : ConfigurationAcknowledgedPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V764_769.IsSupportedVersionStatic(protocolVersion))
                V764_769.SerializeInternal(ref writer, protocolVersion);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.ConfigurationAcknowledged),
                    protocolVersion);
        }
    }
}