using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SetPlayerInventory", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SetPlayerInventoryPacket : IServerPacket
    {
        public int SlotId { get; set; }
        public Slot? Contents { get; set; }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : SetPlayerInventoryPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                SlotId = reader.ReadVarInt();
                Contents = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) => r_0.ReadSlot(protocolVersion));
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}