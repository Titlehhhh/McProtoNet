using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("SetBeaconEffect", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class SetBeaconEffectPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, MinecraftVersion.LatestProtocol)
    };

    public VFirst_758Fields? VFirst_758 { get; set; }
    public V759_LastFields? V759_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                var fields = VFirst_758 ?? throw new InvalidOperationException("SetBeaconEffect VFirst_758 fields missing.");
                writer.WriteVarInt(fields.PrimaryEffect);
                writer.WriteVarInt(fields.SecondaryEffect);
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V759_Last ?? throw new InvalidOperationException("SetBeaconEffect V759_Last fields missing.");
                writer.WriteBoolean(fields.PrimaryEffect is not null);
                if (fields.PrimaryEffect is not null)
                {
                    writer.WriteVarInt(fields.PrimaryEffect.Value);
                }
                writer.WriteBoolean(fields.SecondaryEffect is not null);
                if (fields.SecondaryEffect is not null)
                {
                    writer.WriteVarInt(fields.SecondaryEffect.Value);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.SetBeaconEffect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                VFirst_758 = new VFirst_758Fields
                {
                    PrimaryEffect = reader.ReadVarInt(),
                    SecondaryEffect = reader.ReadVarInt()
                };
                V759_Last = null;
                return;
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                int? primary = reader.ReadBoolean() ? reader.ReadVarInt() : null;
                int? secondary = reader.ReadBoolean() ? reader.ReadVarInt() : null;
                V759_Last = new V759_LastFields
                {
                    PrimaryEffect = primary,
                    SecondaryEffect = secondary
                };
                VFirst_758 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.SetBeaconEffect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_758Fields
    {
        public int PrimaryEffect { get; set; }
        public int SecondaryEffect { get; set; }
    }

    public struct V759_LastFields
    {
        public int? PrimaryEffect { get; set; }
        public int? SecondaryEffect { get; set; }
    }
}
