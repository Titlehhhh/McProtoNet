using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("CraftRecipeRequest", PacketState.Play, PacketDirection.Serverbound)]
    public partial class CraftRecipeRequestPacket : IClientPacket
    {
        public bool MakeAll { get; set; }

        [PacketSubInfo(340, 340)]
        public sealed partial class V340 : CraftRecipeRequestPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId, Recipe, MakeAll);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                sbyte windowId, int recipe, bool makeAll)
            {
                writer.WriteSignedByte(windowId);
                writer.WriteVarInt(recipe);
                writer.WriteBoolean(makeAll);
            }

            public sbyte WindowId { get; set; }
            public int Recipe { get; set; }
        }

        [PacketSubInfo(351, 767)]
        public sealed partial class V351_767 : CraftRecipeRequestPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId, Recipe, MakeAll);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                sbyte windowId, string recipe, bool makeAll)
            {
                writer.WriteSignedByte(windowId);
                writer.WriteString(recipe);
                writer.WriteBoolean(makeAll);
            }

            public sbyte WindowId { get; set; }
            public string Recipe { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : CraftRecipeRequestPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId, RecipeId, MakeAll);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int windowId, int recipeId, bool makeAll)
            {
                writer.WriteVarInt(windowId);
                writer.WriteVarInt(recipeId);
                writer.WriteBoolean(makeAll);
            }

            public int WindowId { get; set; }
            public int RecipeId { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340.IsSupportedVersionStatic(protocolVersion))
                V340.SerializeInternal(ref writer, protocolVersion, 0, 0, MakeAll);
            else if (V351_767.IsSupportedVersionStatic(protocolVersion))
                V351_767.SerializeInternal(ref writer, protocolVersion, 0, string.Empty, MakeAll);
            else if (V768_769.IsSupportedVersionStatic(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, default, 0, MakeAll);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.CraftRecipeRequest), protocolVersion);
        }
    }
}