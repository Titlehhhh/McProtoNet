using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("UpdateCommandBlock", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class UpdateCommandBlockPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)
    };

    public Position Location { get; set; }
    public string Command { get; set; }
    public int Mode { get; set; }
    public byte Flags { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WritePosition(location, protocolVersion);
                writer.WriteString(Command);
                writer.WriteVarInt(Mode);
                writer.WriteUnsignedByte(Flags);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.UpdateCommandBlock), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                location, protocolVersion = reader.ReadPosition(protocolVersion);
                Command = reader.ReadString();
                Mode = reader.ReadVarInt();
                Flags = reader.ReadUnsignedByte();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.UpdateCommandBlock), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
