﻿using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("GameStateChange", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class GameStateChangePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 770),
        new(771, MinecraftVersion.LatestProtocol)
    };

    public byte Reason { get; set; }
    public float GameMode { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
                writer.WriteUnsignedByte(Reason);
                writer.WriteFloat(GameMode);
                return;
            case >= 771 and <= MinecraftVersion.LatestProtocol:
                writer.WriteUnsignedByte(Reason);
                writer.WriteFloat(GameMode);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.GameStateChange), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
                Reason = reader.ReadUnsignedByte();
                GameMode = reader.ReadFloat();
                return;
            case >= 771 and <= MinecraftVersion.LatestProtocol:
                Reason = reader.ReadUnsignedByte();
                GameMode = reader.ReadFloat();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.GameStateChange), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
