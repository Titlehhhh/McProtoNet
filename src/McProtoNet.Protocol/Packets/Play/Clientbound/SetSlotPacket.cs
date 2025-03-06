using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SetSlot", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SetSlotPacket : IServerPacket
    {
        public short Slot { get; set; }
        public Slot? Item { get; set; }

        [PacketSubInfo(340, 755)]
        public sealed partial class V340_755 : SetSlotPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadSignedByte();
                Slot = reader.ReadSignedShort();
                Item = reader.ReadSlot(protocolVersion);
            }

            public sbyte WindowId { get; set; }
        }

        [PacketSubInfo(756, 765)]
        public sealed partial class V756_765 : SetSlotPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadSignedByte();
                StateId = reader.ReadVarInt();
                Slot = reader.ReadSignedShort();
                Item = reader.ReadSlot(protocolVersion);
            }

            public sbyte WindowId { get; set; }
            public int StateId { get; set; }
        }

        [PacketSubInfo(766, 767)]
        public sealed partial class V766_767 : SetSlotPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadSignedByte();
                StateId = reader.ReadVarInt();
                Slot = reader.ReadSignedShort();
                Item = reader.ReadSlot(protocolVersion);
            }

            public sbyte WindowId { get; set; }
            public int StateId { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : SetSlotPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadVarInt();
                StateId = reader.ReadVarInt();
                Slot = reader.ReadSignedShort();
                Item = reader.ReadSlot(protocolVersion);
            }

            public int WindowId { get; set; }
            public int StateId { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}