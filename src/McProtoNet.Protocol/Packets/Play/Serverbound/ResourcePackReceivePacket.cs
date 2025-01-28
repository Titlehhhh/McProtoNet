using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("ResourcePackReceive", PacketState.Play, PacketDirection.Serverbound)]
    public partial class ResourcePackReceivePacket : IClientPacket
    {
        public int Result { get; set; }

        [PacketSubInfo(340, 764)]
        public sealed partial class V340_764 : ResourcePackReceivePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Result);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int result)
            {
                writer.WriteVarInt(result);
            }
        }

        [PacketSubInfo(765, 769)]
        public sealed partial class V765_769 : ResourcePackReceivePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Uuid, Result);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Guid uuid, int result)
            {
                writer.WriteUUID(uuid);
                writer.WriteVarInt(result);
            }

            public Guid Uuid { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_764.IsSupportedVersionStatic(protocolVersion))
                V340_764.SerializeInternal(ref writer, protocolVersion, Result);
            else if (V765_769.IsSupportedVersionStatic(protocolVersion))
                V765_769.SerializeInternal(ref writer, protocolVersion, default, Result);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.ResourcePackReceive), protocolVersion);
        }
    }
}