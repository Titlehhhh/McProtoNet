using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("DisplayedRecipe", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(751, MinecraftVersion.LatestProtocol)]
[PacketId(751, 758, 0x1F)]
[PacketId(759, 759, 0x21)]
[PacketId(760, 763, 0x22)]
[PacketId(764, 764, 0x25)]
[PacketId(765, 765, 0x26)]
[PacketId(766, 767, 0x29)]
[PacketId(768, 768, 0x2B)]
[PacketId(769, 770, 0x2D)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x2E)]
public sealed partial class DisplayedRecipePacket : IClientPacket
{
    public V751_767Fields? V751_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 751 and <= 767:
            {
                var fields = V751_767 ?? throw new InvalidOperationException("DisplayedRecipePacket 751-767 fields missing.");
                writer.WriteString(fields.RecipeId);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("DisplayedRecipePacket 768-last fields missing.");
                writer.WriteVarInt(fields.RecipeId);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(DisplayedRecipePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 751 and <= 767:
            {
                V751_767 = new V751_767Fields { RecipeId = reader.ReadString() };
                V768_Last = null;
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                V768_Last = new V768_LastFields { RecipeId = reader.ReadVarInt() };
                V751_767 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(DisplayedRecipePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V751_767Fields
    {
        public string RecipeId { get; set; }
    }

    public struct V768_LastFields
    {
        public int RecipeId { get; set; }
    }
}