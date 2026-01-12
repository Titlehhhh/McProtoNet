using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CraftProgressBar", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class CraftProgressBarPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 765),
        new(766, MinecraftVersion.LatestProtocol),
    };

    public short Property { get; set; }
    public short Value { get; set; }
    public int WindowId { get; set; }



    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                writer.WriteUnsignedByte(WindowId);
                writer.WriteSignedShort(Property);
                writer.WriteSignedShort(Value);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                if (protocolVersion <= 767)
                {
                    writer.WriteUnsignedByte((byte)WindowId);
                }
                else
                {
                    writer.WriteVarInt(WindowId);
                }
                writer.WriteSignedShort(Property);
                writer.WriteSignedShort(Value);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.CraftProgressBar), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                WindowId = reader.ReadUnsignedByte();
                Property = reader.ReadSignedShort();
                Value = reader.ReadSignedShort();
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                WindowId = protocolVersion <= 767
                    ? reader.ReadUnsignedByte()
                    : reader.ReadVarInt();
                Property = reader.ReadSignedShort();
                Value = reader.ReadSignedShort();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.CraftProgressBar), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
