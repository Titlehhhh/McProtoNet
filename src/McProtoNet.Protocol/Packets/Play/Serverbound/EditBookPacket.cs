using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("EditBook", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class EditBookPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 755),
        new(756, MinecraftVersion.LatestProtocol)
    };

    public int Hand { get; set; }

    public VFirst_755Fields? VFirst_755 { get; set; }
    public V756_LastFields? V756_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 755:
            {
                var fields = VFirst_755 ?? throw new InvalidOperationException("EditBook VFirst_755 fields missing.");
                writer.WriteSlot(fields.NewBook, protocolVersion);
                writer.WriteBoolean(fields.Signing);
                writer.WriteVarInt(Hand);
                return;
            }
            case >= 756 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V756_Last ?? throw new InvalidOperationException("EditBook V756_Last fields missing.");
                writer.WriteVarInt(Hand);
                writer.WriteVarInt(fields.Pages.Length);
                for (int i = 0; i < fields.Pages.Length; i++)
                {
                    writer.WriteString(fields.Pages[i]);
                }
                writer.WriteBoolean(fields.Title is not null);
                if (fields.Title is not null)
                {
                    writer.WriteString(fields.Title);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.EditBook), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 755:
                VFirst_755 = new VFirst_755Fields
                {
                    NewBook = reader.ReadSlot(protocolVersion),
                    Signing = reader.ReadBoolean()
                };
                Hand = reader.ReadVarInt();
                V756_Last = null;
                return;
            case >= 756 and <= MinecraftVersion.LatestProtocol:
            {
                Hand = reader.ReadVarInt();
                int length = reader.ReadVarInt();
                var pages = new string[length];
                for (int i = 0; i < length; i++)
                {
                    pages[i] = reader.ReadString();
                }
                string? title = reader.ReadBoolean() ? reader.ReadString() : null;
                V756_Last = new V756_LastFields
                {
                    Pages = pages,
                    Title = title
                };
                VFirst_755 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.EditBook), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_755Fields
    {
        public Slot NewBook { get; set; }
        public bool Signing { get; set; }
    }

    public struct V756_LastFields
    {
        public string[] Pages { get; set; }
        public string? Title { get; set; }
    }
}
