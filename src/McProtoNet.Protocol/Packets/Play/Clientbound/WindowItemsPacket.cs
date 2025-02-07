using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("WindowItems", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class WindowItemsPacket : IServerPacket
    {
        public Slot[] Items { get; set; }

        [PacketSubInfo(340, 755)]
        public sealed partial class V340_755 : WindowItemsPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadUnsignedByte();
                Items = reader.ReadArray(LengthFormat.Short,
                    (ref MinecraftPrimitiveReader r_0) => r_0.ReadSlot(protocolVersion));
            }

            public byte WindowId { get; set; }
        }

        [PacketSubInfo(756, 765)]
        public sealed partial class V756_765 : WindowItemsPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadUnsignedByte();
                StateId = reader.ReadVarInt();
                Items = reader.ReadArray(LengthFormat.VarInt,
                    (ref MinecraftPrimitiveReader r_0) => r_0.ReadSlot(protocolVersion));
                CarriedItem = reader.ReadSlot(protocolVersion);
            }

            public byte WindowId { get; set; }
            public int StateId { get; set; }
            public Slot CarriedItem { get; set; }
        }

        [PacketSubInfo(766, 767)]
        public sealed partial class V766_767 : WindowItemsPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadUnsignedByte();
                StateId = reader.ReadVarInt();
                Items = reader.ReadArray(LengthFormat.VarInt,
                    (ref MinecraftPrimitiveReader r_0) => r_0.ReadSlot(protocolVersion));
                CarriedItem = reader.ReadSlot(protocolVersion);
            }

            public byte WindowId { get; set; }
            public int StateId { get; set; }
            public Slot CarriedItem { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : WindowItemsPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadVarInt();
                StateId = reader.ReadVarInt();
                Items = reader.ReadArray(LengthFormat.VarInt,
                    (ref MinecraftPrimitiveReader r_0) => r_0.ReadSlot(protocolVersion));
                CarriedItem = reader.ReadSlot(protocolVersion);
            }

            public int WindowId { get; set; }
            public int StateId { get; set; }
            public Slot CarriedItem { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}