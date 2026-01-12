using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityDestroy", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class EntityDestroyPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
        new(756, MinecraftVersion.LatestProtocol),
    };

    public int[] EntityIds { get; set; }



    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                writer.WriteVarInt(EntityIds.Length);
                for (int i = 0; i < EntityIds.Length; i++)
                {
                    writer.WriteVarInt(EntityIds[i]);
                }
                return;
            }
            case >= 756 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(EntityIds.Length);
                for (int i = 0; i < EntityIds.Length; i++)
                {
                    writer.WriteVarInt(EntityIds[i]);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntityDestroy), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                EntityIds = reader.ReadArray<int, VarIntArrayReader>(LengthFormat.VarInt);
                return;
            }
            case >= 756 and <= MinecraftVersion.LatestProtocol:
            {
                EntityIds = reader.ReadArray<int, VarIntArrayReader>(LengthFormat.VarInt);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntityDestroy), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
