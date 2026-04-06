using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;
using McProtoNet.Protocol.Extensions;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("NbtQueryResponse", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class NbtQueryResponsePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 763),
        new(764, MinecraftVersion.LatestProtocol),
    };

    public int TransactionId { get; set; }
    public NbtTag? Nbt { get; set; }



    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
            {
                writer.WriteVarInt(TransactionId);
                writer.WriteOptionalNbtTag(Nbt, protocolVersion);
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(TransactionId);
                writer.WriteAnonOptionalNbtTag(Nbt, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.NbtQueryResponse), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
            {
                TransactionId = reader.ReadVarInt();
                Nbt = reader.ReadOptionalNbtTag(true);
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                TransactionId = reader.ReadVarInt();
                Nbt = reader.ReadOptionalNbtTag(false);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.NbtQueryResponse), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
