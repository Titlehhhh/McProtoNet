using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("HeldItemSlot", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class HeldItemSlotPacket : IServerPacket
    {
        [PacketSubInfo(340, 768)]
        public sealed partial class V340_768 : HeldItemSlotPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Slot = reader.ReadSignedByte();
            }

            public sbyte Slot { get; set; }
        }

        [PacketSubInfo(769, 769)]
        public sealed partial class V769 : HeldItemSlotPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Slot = reader.ReadVarInt();
            }

            public int Slot { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}