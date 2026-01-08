using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("RemoveEntityEffect", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class RemoveEntityEffectPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 757),
        new(758, MinecraftVersion.LatestProtocol),
    };

    public int EntityId { get; set; }
    public sbyte EffectId { get; set; }



    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 757:
            {
                writer.WriteVarInt(EntityId);
                writer.WriteSignedByte(EffectId);
                return;
            }
            case >= 758 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(EntityId);
                writer.WriteVarInt(EffectId);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.RemoveEntityEffect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 757:
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadSignedByte();
                return;
            }
            case >= 758 and <= MinecraftVersion.LatestProtocol:
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadVarInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.RemoveEntityEffect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
