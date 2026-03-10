using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("UseItem", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class UseItemPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, 766),
        new(767, MinecraftVersion.LatestProtocol)
    };

    public int Hand { get; set; }

    public V759_766Fields? V759_766 { get; set; }
    public V767_LastFields? V767_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                writer.WriteVarInt(Hand);
                return;
            case >= 759 and <= 766:
            {
                var fields = V759_766 ?? throw new InvalidOperationException("UseItem V759_766 fields missing.");
                writer.WriteVarInt(Hand);
                writer.WriteVarInt(fields.Sequence);
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V767_Last ?? throw new InvalidOperationException("UseItem V767_Last fields missing.");
                writer.WriteVarInt(Hand);
                writer.WriteVarInt(fields.Sequence);
                writer.WriteVector2(fields.Rotation, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.UseItem), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                Hand = reader.ReadVarInt();
                V759_766 = null;
                V767_Last = null;
                return;
            case >= 759 and <= 766:
                Hand = reader.ReadVarInt();
                V759_766 = new V759_766Fields
                {
                    Sequence = reader.ReadVarInt()
                };
                V767_Last = null;
                return;
            case >= 767 and <= MinecraftVersion.LatestProtocol:
                Hand = reader.ReadVarInt();
                V767_Last = new V767_LastFields
                {
                    Sequence = reader.ReadVarInt(),
                    Rotation = reader.ReadVector2(protocolVersion)
                };
                V759_766 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.UseItem), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759_766Fields
    {
        public int Sequence { get; set; }
    }

    public struct V767_LastFields
    {
        public int Sequence { get; set; }
        public Vector2 Rotation { get; set; }
    }
}
