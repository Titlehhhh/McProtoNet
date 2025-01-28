using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("BlockBreakAnimation", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class BlockBreakAnimationPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public Position Location { get; set; }
        public sbyte DestroyStage { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : BlockBreakAnimationPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                Location = reader.ReadPosition(protocolVersion);
                DestroyStage = reader.ReadSignedByte();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}