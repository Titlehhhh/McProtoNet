using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class CraftRecipeRequestPacket : IClientPacket
    {
        public bool MakeAll { get; set; }

        public sealed class V340 : CraftRecipeRequestPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId, Recipe, MakeAll);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, sbyte windowId, int recipe, bool makeAll)
            {
                writer.WriteSignedByte(windowId);
                writer.WriteVarInt(recipe);
                writer.WriteBoolean(makeAll);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion == 340;
            }

            public sbyte WindowId { get; set; }
            public int Recipe { get; set; }
        }

        public sealed class V351_767 : CraftRecipeRequestPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId, Recipe, MakeAll);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, sbyte windowId, string recipe, bool makeAll)
            {
                writer.WriteSignedByte(windowId);
                writer.WriteString(recipe);
                writer.WriteBoolean(makeAll);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 351 and <= 767;
            }

            public sbyte WindowId { get; set; }
            public string Recipe { get; set; }
        }

        public sealed class V768_769 : CraftRecipeRequestPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId, RecipeId, MakeAll);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int windowId, int recipeId, bool makeAll)
            {
                writer.WriteVarInt(windowId);
                writer.WriteVarInt(recipeId);
                writer.WriteBoolean(makeAll);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }

            public int WindowId { get; set; }
            public int RecipeId { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340.SupportedVersion(protocolVersion) || V351_767.SupportedVersion(protocolVersion) || V768_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340.SupportedVersion(protocolVersion))
                V340.SerializeInternal(ref writer, protocolVersion, 0, default, MakeAll);
            else if (V351_767.SupportedVersion(protocolVersion))
                V351_767.SerializeInternal(ref writer, protocolVersion, 0, default, MakeAll);
            else if (V768_769.SupportedVersion(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, default, default, MakeAll);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.CraftRecipeRequest), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.CraftRecipeRequest;

        public ClientPacket GetPacketId() => PacketId;
    }
}