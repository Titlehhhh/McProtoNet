using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class RecipeBookRemovePacket : IServerPacket
    {
        public int[] RecipeIds { get; set; }

        public sealed class V768_769 : RecipeBookRemovePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                RecipeIds = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.VarInt);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V768_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.RecipeBookRemove;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}