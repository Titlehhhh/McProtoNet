using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("ClientCommand", PacketState.Play, PacketDirection.Serverbound)]
    public partial class ClientCommandPacket : IClientPacket
    {
        public int ActionId { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : ClientCommandPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, ActionId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int actionId)
            {
                writer.WriteVarInt(actionId);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_769.IsSupportedVersionStatic(protocolVersion))
                V340_769.SerializeInternal(ref writer, protocolVersion, ActionId);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.ClientCommand), protocolVersion);
        }
    }
}