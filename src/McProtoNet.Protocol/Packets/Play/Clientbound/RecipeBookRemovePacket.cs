using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("RecipeBookRemove", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class RecipeBookRemovePacket : IServerPacket
    {
        public int[] RecipeIds { get; set; }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : RecipeBookRemovePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                RecipeIds = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.VarInt);
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}