using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("CraftingBookData", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 736)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1E)]
public sealed partial class CraftingBookDataPacket : IClientPacket
{
    public int Type { get; set; }
    public string? DisplayedRecipe { get; set; }
    public bool? CraftingBookOpen { get; set; }
    public bool? CraftingFilter { get; set; }
    public bool? SmeltingBookOpen { get; set; }
    public bool? SmeltingFilter { get; set; }
    public bool? BlastingBookOpen { get; set; }
    public bool? BlastingFilter { get; set; }
    public bool? SmokingBookOpen { get; set; }
    public bool? SmokingFilter { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Type);
        switch (Type)
        {
            case 0:
                writer.WriteString(DisplayedRecipe!);
                break;
            case 1:
                writer.WriteBoolean(CraftingBookOpen!.Value);
                writer.WriteBoolean(CraftingFilter!.Value);
                writer.WriteBoolean(SmeltingBookOpen!.Value);
                writer.WriteBoolean(SmeltingFilter!.Value);
                writer.WriteBoolean(BlastingBookOpen!.Value);
                writer.WriteBoolean(BlastingFilter!.Value);
                writer.WriteBoolean(SmokingBookOpen!.Value);
                writer.WriteBoolean(SmokingFilter!.Value);
                break;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CraftingBookDataPacket), protocolVersion, SupportedVersions);
                break;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Type = reader.ReadVarInt();
        switch (Type)
        {
            case 0:
                DisplayedRecipe = reader.ReadString();
                break;
            case 1:
                CraftingBookOpen = reader.ReadBoolean();
                CraftingFilter = reader.ReadBoolean();
                SmeltingBookOpen = reader.ReadBoolean();
                SmeltingFilter = reader.ReadBoolean();
                BlastingBookOpen = reader.ReadBoolean();
                BlastingFilter = reader.ReadBoolean();
                SmokingBookOpen = reader.ReadBoolean();
                SmokingFilter = reader.ReadBoolean();
                break;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CraftingBookDataPacket), protocolVersion, SupportedVersions);
                break;
        }
    }
}