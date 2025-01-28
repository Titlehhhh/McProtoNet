using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SetTickingState", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SetTickingStatePacket : IServerPacket
    {
        public float TickRate { get; set; }
        public bool IsFrozen { get; set; }

        [PacketSubInfo(765, 769)]
        public sealed partial class V765_769 : SetTickingStatePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                TickRate = reader.ReadFloat();
                IsFrozen = reader.ReadBoolean();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}