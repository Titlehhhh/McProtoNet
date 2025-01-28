using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("CloseWindow", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class CloseWindowPacket : IServerPacket
    {
        [PacketSubInfo(340, 767)]
        public sealed partial class V340_767 : CloseWindowPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadUnsignedByte();
            }

            public byte WindowId { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : CloseWindowPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadVarInt();
            }

            public int WindowId { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}