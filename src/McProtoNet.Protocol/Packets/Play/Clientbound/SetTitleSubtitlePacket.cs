using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetTitleSubtitle", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SetTitleSubtitlePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(755, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public VFirst_764Fields? VFirst_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 764:
            {
                var fields = VFirst_764 ?? throw new InvalidOperationException("SetTitleSubtitle VFirst_764 missing.");
                writer.WriteString(fields.Text);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("SetTitleSubtitle V765_Last missing.");
                writer.WriteAnonymousNbtTag(fields.Text, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetTitleSubtitle), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 764:
            {
                var fields = new VFirst_764Fields
                {
                    Text = reader.ReadString()
                };
                VFirst_764 = fields;
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V765_LastFields
                {
                    Text = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("SetTitleSubtitle Text missing.")
                };
                V765_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetTitleSubtitle), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_764Fields
    {
        public string Text { get; set; }
    }

    public struct V765_LastFields
    {
        public NbtTag Text { get; set; }
    }
}
