using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;
using McProtoNet.Protocol.Extensions;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("PlayerlistHeader", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class PlayerlistHeaderPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public string Header { get; set; }
    public string Footer { get; set; }



    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                writer.WriteString(Header);
                writer.WriteString(Footer);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteAnonymousNbtTag(Header ?? throw new InvalidOperationException("PlayerlistHeader Header missing."), protocolVersion);
                writer.WriteAnonymousNbtTag(Footer ?? throw new InvalidOperationException("PlayerlistHeader Footer missing."), protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.PlayerlistHeader), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                Header = reader.ReadString();
                Footer = reader.ReadString();
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                Header = reader.ReadNbtTag(false);
                Footer = reader.ReadNbtTag(false);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.PlayerlistHeader), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
