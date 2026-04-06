using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("HideMessage", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class HideMessagePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(760, 760),
        new(761, MinecraftVersion.LatestProtocol),
    };

    public V760Fields? V760 { get; set; }
    public V761_LastFields? V761_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 760:
            {
                var fields = V760 ?? throw new InvalidOperationException("HideMessage V760 fields missing.");
                writer.WriteBuffer<VarInt>(fields.Signature);
                return;
            }
            case >= 761 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V761_Last ?? throw new InvalidOperationException("HideMessage V761_Last fields missing.");
                writer.WriteVarInt(fields.Id);
                if (fields.Id == 0)
                {
                    writer.WriteBuffer(fields.Signature ?? throw new InvalidOperationException("HideMessage signature missing."), 256);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.HideMessage), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 760:
                V760 = new V760Fields
                {
                    Signature = reader.ReadBuffer(LengthFormat.VarInt)
                };
                return;
            case >= 761 and <= MinecraftVersion.LatestProtocol:
            {
                int id = reader.ReadVarInt();
                var fields = new V761_LastFields
                {
                    Id = id
                };
                if (id == 0)
                {
                    fields.Signature = reader.ReadBuffer(256);
                }
                V761_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.HideMessage), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V760Fields
    {
        public byte[] Signature { get; set; }
    }

    public struct V761_LastFields
    {
        public int Id { get; set; }
        public byte[]? Signature { get; set; }
    }
}
