using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("CraftProgressBar", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class CraftProgressBarPacket : IServerPacket
    {
        public short Property { get; set; }
        public short Value { get; set; }

        [PacketSubInfo(340, 767)]
        public sealed partial class V340_767 : CraftProgressBarPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadUnsignedByte();
                Property = reader.ReadSignedShort();
                Value = reader.ReadSignedShort();
            }

            public byte WindowId { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : CraftProgressBarPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadVarInt();
                Property = reader.ReadSignedShort();
                Value = reader.ReadSignedShort();
            }

            public int WindowId { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}