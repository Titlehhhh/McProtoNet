using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("StepTick", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class StepTickPacket : IServerPacket
    {
        public int TickSteps { get; set; }

        [PacketSubInfo(765, 769)]
        public sealed partial class V765_769 : StepTickPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                TickSteps = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}