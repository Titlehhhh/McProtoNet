using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("TeleportConfirm", PacketState.Play, PacketDirection.Serverbound)]
    public partial class TeleportConfirmPacket : IClientPacket
    {
        public int TeleportId { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : TeleportConfirmPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, TeleportId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int teleportId)
            {
                writer.WriteVarInt(teleportId);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_769.IsSupportedVersionStatic(protocolVersion))
                V340_769.SerializeInternal(ref writer, protocolVersion, TeleportId);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.TeleportConfirm), protocolVersion);
        }
    }
}