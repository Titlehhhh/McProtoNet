using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("DisplayedRecipe", PacketState.Play, PacketDirection.Serverbound)]
    public partial class DisplayedRecipePacket : IClientPacket
    {
        [PacketSubInfo(751, 767)]
        public sealed partial class V751_767 : DisplayedRecipePacket
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

            public string RecipeId { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : DisplayedRecipePacket
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

            public int RecipeId { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V751_767.IsSupportedVersionStatic(protocolVersion))
                V751_767.SerializeInternal(ref writer, protocolVersion, string.Empty);
            else if (V768_769.IsSupportedVersionStatic(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, 0);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.DisplayedRecipe), protocolVersion);
        }
    }
}