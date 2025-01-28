using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("DeathCombatEvent", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class DeathCombatEventPacket : IServerPacket
    {
        public int PlayerId { get; set; }

        [PacketSubInfo(755, 762)]
        public sealed partial class V755_762 : DeathCombatEventPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                PlayerId = reader.ReadVarInt();
                EntityId = reader.ReadSignedInt();
                Message = reader.ReadString();
            }

            public int EntityId { get; set; }
            public string Message { get; set; }
        }

        [PacketSubInfo(763, 764)]
        public sealed partial class V763_764 : DeathCombatEventPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                PlayerId = reader.ReadVarInt();
                Message = reader.ReadString();
            }

            public string Message { get; set; }
        }

        [PacketSubInfo(765, 769)]
        public sealed partial class V765_769 : DeathCombatEventPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                PlayerId = reader.ReadVarInt();
                Message = reader.ReadNbtTag(false);
            }

            public NbtTag Message { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}