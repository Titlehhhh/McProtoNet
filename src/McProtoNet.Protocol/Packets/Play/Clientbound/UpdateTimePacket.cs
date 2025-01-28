using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("UpdateTime", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class UpdateTimePacket : IServerPacket
    {
        public long Age { get; set; }
        public long Time { get; set; }

        [PacketSubInfo(340, 767)]
        public sealed partial class V340_767 : UpdateTimePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Age = reader.ReadSignedLong();
                Time = reader.ReadSignedLong();
            }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : UpdateTimePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Age = reader.ReadSignedLong();
                Time = reader.ReadSignedLong();
                TickDayTime = reader.ReadBoolean();
            }

            public bool TickDayTime { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}