using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;
using McProtoNet.Protocol.Extensions;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("AddResourcePack", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class AddResourcePackPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(765, 765),
    };

    public Guid Uuid { get; set; }
    public string Url { get; set; }
    public string Hash { get; set; }
    public bool Forced { get; set; }
    public NbtTag? PromptMessage { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 765 and <= 765:
                writer.WriteUUID(Uuid);
                writer.WriteString(Url);
                writer.WriteString(Hash);
                writer.WriteBoolean(Forced);
                writer.WriteAnonOptionalNbtTag(PromptMessage, protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.AddResourcePack), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 765 and <= 765:
                Uuid = reader.ReadUUID();
                Url = reader.ReadString();
                Hash = reader.ReadString();
                Forced = reader.ReadBoolean();
                PromptMessage = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadNbtTag(false));
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.AddResourcePack), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
