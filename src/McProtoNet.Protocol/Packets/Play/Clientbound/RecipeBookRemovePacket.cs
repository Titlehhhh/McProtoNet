using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("RecipeBookRemove", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class RecipeBookRemovePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(768, MinecraftVersion.LatestProtocol),
    };

    public int[] RecipeIds { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(RecipeIds.Length);
                for (int i = 0; i < RecipeIds.Length; i++)
                {
                    writer.WriteVarInt(RecipeIds[i]);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.RecipeBookRemove), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                RecipeIds = reader.ReadArray<int, VarIntArrayReader>(LengthFormat.VarInt);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.RecipeBookRemove), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
