using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("RecipeBook", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class RecipeBookPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(751, MinecraftVersion.LatestProtocol)
    };

    public int BookId { get; set; }
    public bool BookOpen { get; set; }
    public bool FilterActive { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 751 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(BookId);
                writer.WriteBoolean(BookOpen);
                writer.WriteBoolean(FilterActive);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.RecipeBook), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 751 and <= MinecraftVersion.LatestProtocol:
                BookId = reader.ReadVarInt();
                BookOpen = reader.ReadBoolean();
                FilterActive = reader.ReadBoolean();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.RecipeBook), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
