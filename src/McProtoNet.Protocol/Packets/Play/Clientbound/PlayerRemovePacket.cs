using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("PlayerRemove", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class PlayerRemovePacket : IServerPacket
    {
        public Guid[] Players { get; set; }

        [PacketSubInfo(761, 769)]
        public sealed partial class V761_769 : PlayerRemovePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Players = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r_0) => r_0.ReadUUID());
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}