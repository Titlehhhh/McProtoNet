using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("CloseWindow", PacketState.Play, PacketDirection.Serverbound)]
    public partial class CloseWindowPacket : IClientPacket
    {
        [PacketSubInfo(340, 767)]
        public sealed partial class V340_767 : CloseWindowPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, byte windowId)
            {
                writer.WriteUnsignedByte(windowId);
            }

            public byte WindowId { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : CloseWindowPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int windowId)
            {
                writer.WriteVarInt(windowId);
            }

            public int WindowId { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_767.IsSupportedVersionStatic(protocolVersion))
                V340_767.SerializeInternal(ref writer, protocolVersion, 0);
            else if (V768_769.IsSupportedVersionStatic(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, default);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.CloseWindow), protocolVersion);
        }
    }
}