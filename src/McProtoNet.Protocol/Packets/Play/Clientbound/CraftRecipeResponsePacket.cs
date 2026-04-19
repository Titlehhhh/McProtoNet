using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CraftRecipeResponse", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x30)]
[PacketId(751, 754, 0x2F)]
[PacketId(755, 758, 0x31)]
[PacketId(759, 759, 0x2E)]
[PacketId(760, 760, 0x30)]
[PacketId(761, 761, 0x2F)]
[PacketId(762, 763, 0x33)]
[PacketId(764, 765, 0x35)]
[PacketId(766, 767, 0x37)]
[PacketId(768, 769, 0x39)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x38)]
public sealed partial class CraftRecipeResponsePacket : IServerPacket
{
    public VFirst_765Fields? VFirst_765 { get; set; }
    public V766_767Fields? V766_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                var fields = VFirst_765 ?? throw new InvalidOperationException("CraftRecipeResponsePacket 1-765 fields missing.");
                writer.WriteSignedByte(fields.WindowId);
                writer.WriteString(fields.Recipe);
                return;
            }
            case >= 766 and <= 767:
            {
                var fields = V766_767 ?? throw new InvalidOperationException("CraftRecipeResponsePacket 766-767 fields missing.");
                writer.WriteUnsignedByte(fields.WindowId);
                writer.WriteString(fields.Recipe);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("CraftRecipeResponsePacket 768-last fields missing.");
                writer.WriteVarInt(fields.WindowId);
                writer.WriteType<RecipeDisplay>(fields.RecipeDisplay, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CraftRecipeResponsePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                VFirst_765 = new VFirst_765Fields
                {
                    WindowId = reader.ReadSignedByte(),
                    Recipe = reader.ReadString()
                };
                V766_767 = null;
                V768_Last = null;
                return;
            }
            case >= 766 and <= 767:
            {
                V766_767 = new V766_767Fields
                {
                    WindowId = reader.ReadUnsignedByte(),
                    Recipe = reader.ReadString()
                };
                VFirst_765 = null;
                V768_Last = null;
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                V768_Last = new V768_LastFields
                {
                    WindowId = reader.ReadVarInt(),
                    RecipeDisplay = reader.ReadType<RecipeDisplay>(protocolVersion)
                };
                VFirst_765 = null;
                V766_767 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CraftRecipeResponsePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct VFirst_765Fields
    {
        public sbyte WindowId { get; set; }
        public string Recipe { get; set; }
    }

    public struct V766_767Fields
    {
        public byte WindowId { get; set; }
        public string Recipe { get; set; }
    }

    public struct V768_LastFields
    {
        public int WindowId { get; set; }
        public RecipeDisplay RecipeDisplay { get; set; }
    }
}