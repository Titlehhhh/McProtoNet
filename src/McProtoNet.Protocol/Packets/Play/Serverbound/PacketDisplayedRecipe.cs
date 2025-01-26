using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class DisplayedRecipePacket : IClientPacket
    {
        public sealed class V751_767 : DisplayedRecipePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, RecipeId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                string recipeId)
            {
                writer.WriteString(recipeId);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 751 and <= 767;
            }

            public string RecipeId { get; set; }
        }

        public sealed class V768_769 : DisplayedRecipePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, RecipeId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int recipeId)
            {
                writer.WriteVarInt(recipeId);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }

            public int RecipeId { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V751_767.SupportedVersion(protocolVersion) || V768_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V751_767.SupportedVersion(protocolVersion))
                V751_767.SerializeInternal(ref writer, protocolVersion, string.Empty);
            else if (V768_769.SupportedVersion(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, 0);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.DisplayedRecipe), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.DisplayedRecipe;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}