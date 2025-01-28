using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("BlockAction", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class BlockActionPacket : IServerPacket
    {
        public Position Location { get; set; }
        public byte Byte1 { get; set; }
        public byte Byte2 { get; set; }
        public int BlockId { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : BlockActionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                Byte1 = reader.ReadUnsignedByte();
                Byte2 = reader.ReadUnsignedByte();
                BlockId = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}