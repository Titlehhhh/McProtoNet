using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("PickItem", PacketState.Play, PacketDirection.Serverbound)]
    public partial class PickItemPacket : IClientPacket
    {
        public int Slot { get; set; }

        [PacketSubInfo(393, 768)]
        public sealed partial class V393_768 : PickItemPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Slot);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int slot)
            {
                writer.WriteVarInt(slot);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_768.IsSupportedVersionStatic(protocolVersion))
                V393_768.SerializeInternal(ref writer, protocolVersion, Slot);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.PickItem), protocolVersion);
        }
    }
}