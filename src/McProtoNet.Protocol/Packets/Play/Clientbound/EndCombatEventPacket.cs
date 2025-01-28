using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("EndCombatEvent", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class EndCombatEventPacket : IServerPacket
    {
        public int Duration { get; set; }

        [PacketSubInfo(755, 762)]
        public sealed partial class V755_762 : EndCombatEventPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Duration = reader.ReadVarInt();
                EntityId = reader.ReadSignedInt();
            }

            public int EntityId { get; set; }
        }

        [PacketSubInfo(763, 769)]
        public sealed partial class V763_769 : EndCombatEventPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Duration = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}