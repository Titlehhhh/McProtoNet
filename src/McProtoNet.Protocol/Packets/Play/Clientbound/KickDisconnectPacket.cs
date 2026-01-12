using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;
using McProtoNet.Protocol.Extensions;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("KickDisconnect", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class KickDisconnectPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public string Reason { get; set; }



    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                writer.WriteString(Reason);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteAnonymousNbtTag(Reason ?? throw new InvalidOperationException("KickDisconnect Reason missing."), protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.KickDisconnect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                Reason = reader.ReadString();
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                Reason = reader.ReadNbtTag(false);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.KickDisconnect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
