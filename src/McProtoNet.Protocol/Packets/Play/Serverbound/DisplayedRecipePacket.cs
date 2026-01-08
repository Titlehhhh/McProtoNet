using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("DisplayedRecipe", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class DisplayedRecipePacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(751, 767),
        new(768, MinecraftVersion.LatestProtocol)
    };

    public V751_767Fields? V751_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 751 and <= 767:
            {
                var fields = V751_767 ?? throw new InvalidOperationException("DisplayedRecipe V751_767 fields missing.");
                writer.WriteString(fields.RecipeId);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("DisplayedRecipe V768_Last fields missing.");
                writer.WriteVarInt(fields.RecipeId);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.DisplayedRecipe), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 751 and <= 767:
                V751_767 = new V751_767Fields
                {
                    RecipeId = reader.ReadString()
                };
                V768_Last = null;
                return;
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                V768_Last = new V768_LastFields
                {
                    RecipeId = reader.ReadVarInt()
                };
                V751_767 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.DisplayedRecipe), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V751_767Fields
    {
        public string RecipeId { get; set; }
    }

    public struct V768_LastFields
    {
        public int RecipeId { get; set; }
    }
}
