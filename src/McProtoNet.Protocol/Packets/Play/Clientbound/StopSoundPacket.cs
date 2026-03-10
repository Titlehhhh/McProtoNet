using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("StopSound", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class StopSoundPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol),
    };

    public sbyte Flags { get; set; }
    public int? Source { get; set; }
    public string? Sound { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteSignedByte(Flags);
                switch (Flags)
                {
                    case 1:
                        writer.WriteVarInt(Source ?? throw new InvalidOperationException("StopSound source missing."));
                        break;
                    case 2:
                        writer.WriteString(Sound ?? throw new InvalidOperationException("StopSound sound missing."));
                        break;
                    case 3:
                        writer.WriteVarInt(Source ?? throw new InvalidOperationException("StopSound source missing."));
                        writer.WriteString(Sound ?? throw new InvalidOperationException("StopSound sound missing."));
                        break;
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.StopSound), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                Flags = reader.ReadSignedByte();
                switch (Flags)
                {
                    case 1:
                        Source = reader.ReadVarInt();
                        break;
                    case 2:
                        Sound = reader.ReadString();
                        break;
                    case 3:
                        Source = reader.ReadVarInt();
                        Sound = reader.ReadString();
                        break;
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.StopSound), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
