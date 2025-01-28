using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("TileEntityData", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class TileEntityDataPacket : IServerPacket
    {
        public Position Location { get; set; }
        public NbtTag? NbtData { get; set; }

        [PacketSubInfo(340, 756)]
        public sealed partial class V340_756 : TileEntityDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                Action = reader.ReadUnsignedByte();
                NbtData = reader.ReadOptionalNbtTag(true);
            }

            public byte Action { get; set; }
        }

        [PacketSubInfo(757, 763)]
        public sealed partial class V757_763 : TileEntityDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                Action = reader.ReadVarInt();
                NbtData = reader.ReadOptionalNbtTag(true);
            }

            public int Action { get; set; }
        }

        [PacketSubInfo(764, 769)]
        public sealed partial class V764_769 : TileEntityDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                Action = reader.ReadVarInt();
                NbtData = reader.ReadOptionalNbtTag(false);
            }

            public int Action { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}