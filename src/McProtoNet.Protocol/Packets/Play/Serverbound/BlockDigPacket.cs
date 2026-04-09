using System;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("BlockDig", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class BlockDigPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, MinecraftVersion.LatestProtocol)
    };

    public int Status { get; set; }
    public Position Location { get; set; }
    public sbyte Face { get; set; }

    public V759_LastFields? V759_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                writer.WriteVarInt(Status);
                writer.WritePosition(Location, protocolVersion);
                writer.WriteSignedByte(Face);
                return;
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V759_Last ?? throw new InvalidOperationException("BlockDig V759_Last fields missing.");
                writer.WriteVarInt(Status);
                writer.WritePosition(Location, protocolVersion);
                writer.WriteSignedByte(Face);
                writer.WriteVarInt(fields.Sequence);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.BlockDig), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                Status = reader.ReadVarInt();
                Location = reader.ReadPosition(protocolVersion);
                Face = reader.ReadSignedByte();
                V759_Last = null;
                return;
            case >= 759 and <= MinecraftVersion.LatestProtocol:
                Status = reader.ReadVarInt();
                Location = reader.ReadPosition(protocolVersion);
                Face = reader.ReadSignedByte();
                V759_Last = new V759_LastFields
                {
                    Sequence = reader.ReadVarInt()
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.BlockDig), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759_LastFields
    {
        public int Sequence { get; set; }
    }
}
