using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ShowDialog", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ShowDialogPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(771, MinecraftVersion.LatestProtocol),
    };

    public DialogHolder Dialog { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 771 and <= MinecraftVersion.LatestProtocol:
                writer.WriteBoolean(Dialog.HasInline);
                if (Dialog.HasInline)
                {
                    writer.WriteAnonymousNbtTag(Dialog.Data ?? throw new InvalidOperationException("ShowDialog dialog data missing."), protocolVersion);
                }
                else
                {
                    writer.WriteVarInt(Dialog.RegistryId ?? throw new InvalidOperationException("ShowDialog registry id missing."));
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ShowDialog), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                bool hasInline = reader.ReadBoolean();
                if (hasInline)
                {
                    Dialog = new DialogHolder
                    {
                        HasInline = true,
                        Data = reader.ReadAnonymousNbtTag(protocolVersion)
                            ?? throw new InvalidOperationException("ShowDialog dialog data missing.")
                    };
                }
                else
                {
                    Dialog = new DialogHolder
                    {
                        HasInline = false,
                        RegistryId = reader.ReadVarInt()
                    };
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ShowDialog), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct DialogHolder
    {
        public bool HasInline { get; set; }
        public int? RegistryId { get; set; }
        public NbtTag? Data { get; set; }
    }
}
