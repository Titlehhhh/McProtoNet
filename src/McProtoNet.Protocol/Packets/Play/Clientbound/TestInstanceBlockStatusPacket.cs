using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("TestInstanceBlockStatus", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class TestInstanceBlockStatusPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(770, MinecraftVersion.LatestProtocol),
    };

    public NbtTag Status { get; set; }
    public Vec3i? Size { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 770 and <= MinecraftVersion.LatestProtocol:
                writer.WriteAnonymousNbtTag(Status, protocolVersion);
                if (Size is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteVec3i(Size.Value, protocolVersion);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.TestInstanceBlockStatus), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 770 and <= MinecraftVersion.LatestProtocol:
                Status = reader.ReadAnonymousNbtTag(protocolVersion)
                    ?? throw new InvalidOperationException("TestInstanceBlockStatus status missing.");
                Size = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadVec3i(protocolVersion));
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.TestInstanceBlockStatus), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
