using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("OpenHorseWindow", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class OpenHorseWindowPacket : IServerPacket
    {
        public int NbSlots { get; set; }
        public int EntityId { get; set; }

        [PacketSubInfo(477, 767)]
        public sealed partial class V477_767 : OpenHorseWindowPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadUnsignedByte();
                NbSlots = reader.ReadVarInt();
                EntityId = reader.ReadSignedInt();
            }

            public byte WindowId { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : OpenHorseWindowPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadVarInt();
                NbSlots = reader.ReadVarInt();
                EntityId = reader.ReadSignedInt();
            }

            public int WindowId { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}