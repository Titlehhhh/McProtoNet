using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CraftRecipeResponse", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class CraftRecipeResponsePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 765),
        new(766, 767),
        new(768, MinecraftVersion.LatestProtocol),
    };

    public int WindowId { get; set; }

    public VFirst_765Fields? VFirst_765 { get; set; }
    public V766_767Fields? V766_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                var fields = VFirst_765 ?? throw new InvalidOperationException("CraftRecipeResponse VFirst_765 fields missing.");
                writer.WriteSignedByte((sbyte)WindowId);
                writer.WriteString(fields.Recipe);
                return;
            }
            case >= 766 and <= 767:
            {
                var fields = V766_767 ?? throw new InvalidOperationException("CraftRecipeResponse V766_767 fields missing.");
                writer.WriteUnsignedByte((byte)WindowId);
                writer.WriteString(fields.Recipe);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("CraftRecipeResponse V768_Last fields missing.");
                writer.WriteVarInt(WindowId);
                writer.WriteRecipeDisplay(fields.RecipeDisplay, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.CraftRecipeResponse), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                WindowId = reader.ReadSignedByte();
                VFirst_765 = new VFirst_765Fields
                {
                    Recipe = reader.ReadString()
                };
                return;
            case >= 766 and <= 767:
                WindowId = reader.ReadUnsignedByte();
                V766_767 = new V766_767Fields
                {
                    Recipe = reader.ReadString()
                };
                return;
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                WindowId = reader.ReadVarInt();
                V768_Last = new V768_LastFields
                {
                    RecipeDisplay = reader.ReadRecipeDisplay(protocolVersion)
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.CraftRecipeResponse), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_765Fields
    {
        public string Recipe { get; set; }
    }

    public struct V766_767Fields
    {
        public string Recipe { get; set; }
    }

    public struct V768_LastFields
    {
        public RecipeDisplay RecipeDisplay { get; set; }
    }
}
