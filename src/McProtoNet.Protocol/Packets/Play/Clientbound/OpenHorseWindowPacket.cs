using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("OpenHorseWindow", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class OpenHorseWindowPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 765),
        new(766, MinecraftVersion.LatestProtocol),
    };

    public int NbSlots { get; set; }
    public int EntityId { get; set; }
    public int WindowId { get; set; }



    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                writer.WriteUnsignedByte(WindowId);
                writer.WriteVarInt(NbSlots);
                writer.WriteSignedInt(EntityId);
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
                writer.WriteVarInt(NbSlots);
                writer.WriteSignedInt(EntityId);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.OpenHorseWindow), protocolVersion, SupportedVersionsStatic);
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
                NbSlots = reader.ReadVarInt();
                EntityId = reader.ReadSignedInt();
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                WindowId = protocolVersion <= 767
                    ? reader.ReadUnsignedByte()
                    : reader.ReadVarInt();
                NbSlots = reader.ReadVarInt();
                EntityId = reader.ReadSignedInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.OpenHorseWindow), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
