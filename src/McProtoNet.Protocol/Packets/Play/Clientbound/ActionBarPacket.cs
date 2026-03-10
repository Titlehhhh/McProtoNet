using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;
using McProtoNet.Protocol.Extensions;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ActionBar", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ActionBarPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(755, 764),
        new(765, 772),
    };

    public string Text { get; set; }



    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 764:
            {
                writer.WriteString(Text);
                return;
            }
            case >= 765 and <= 772:
            {
                writer.WriteAnonymousNbtTag(Text ?? throw new InvalidOperationException("ActionBar Text missing."), protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ActionBar), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 764:
            {
                Text = reader.ReadString();
                return;
            }
            case >= 765 and <= 772:
            {
                Text = reader.ReadNbtTag(false);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ActionBar), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
