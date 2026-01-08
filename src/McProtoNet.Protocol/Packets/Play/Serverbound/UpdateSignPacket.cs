using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("UpdateSign", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class UpdateSignPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 762),
        new(763, MinecraftVersion.LatestProtocol)
    };

    public Position Location { get; set; }
    public string Text1 { get; set; } = string.Empty;
    public string Text2 { get; set; } = string.Empty;
    public string Text3 { get; set; } = string.Empty;
    public string Text4 { get; set; } = string.Empty;

    public V763_LastFields? V763_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
                writer.WritePosition(Location, protocolVersion);
                writer.WriteString(Text1);
                writer.WriteString(Text2);
                writer.WriteString(Text3);
                writer.WriteString(Text4);
                return;
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V763_Last ?? throw new InvalidOperationException("UpdateSign V763_Last fields missing.");
                writer.WritePosition(Location, protocolVersion);
                writer.WriteBoolean(fields.IsFrontText);
                writer.WriteString(Text1);
                writer.WriteString(Text2);
                writer.WriteString(Text3);
                writer.WriteString(Text4);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.UpdateSign), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
                Location = reader.ReadPosition(protocolVersion);
                Text1 = reader.ReadString();
                Text2 = reader.ReadString();
                Text3 = reader.ReadString();
                Text4 = reader.ReadString();
                V763_Last = null;
                return;
            case >= 763 and <= MinecraftVersion.LatestProtocol:
                Location = reader.ReadPosition(protocolVersion);
                V763_Last = new V763_LastFields
                {
                    IsFrontText = reader.ReadBoolean()
                };
                Text1 = reader.ReadString();
                Text2 = reader.ReadString();
                Text3 = reader.ReadString();
                Text4 = reader.ReadString();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.UpdateSign), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V763_LastFields
    {
        public bool IsFrontText { get; set; }
    }
}
