using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SoundEffect", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SoundEffectPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, 760),
        new(761, MinecraftVersion.LatestProtocol),
    };

    public VFirst_758Fields? VFirst_758 { get; set; }
    public V759_760Fields? V759_760 { get; set; }
    public V761_LastFields? V761_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                var fields = VFirst_758 ?? throw new InvalidOperationException("SoundEffect VFirst_758 fields missing.");
                writer.WriteVarInt(fields.SoundId);
                writer.WriteVarInt(fields.SoundCategory);
                writer.WriteSignedInt(fields.X);
                writer.WriteSignedInt(fields.Y);
                writer.WriteSignedInt(fields.Z);
                writer.WriteFloat(fields.Volume);
                writer.WriteFloat(fields.Pitch);
                return;
            }
            case >= 759 and <= 760:
            {
                var fields = V759_760 ?? throw new InvalidOperationException("SoundEffect V759_760 fields missing.");
                writer.WriteVarInt(fields.SoundId);
                writer.WriteVarInt(fields.SoundCategory);
                writer.WriteSignedInt(fields.X);
                writer.WriteSignedInt(fields.Y);
                writer.WriteSignedInt(fields.Z);
                writer.WriteFloat(fields.Volume);
                writer.WriteFloat(fields.Pitch);
                writer.WriteSignedLong(fields.Seed);
                return;
            }
            case >= 761 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V761_Last ?? throw new InvalidOperationException("SoundEffect V761_Last fields missing.");
                writer.WriteItemSoundHolder(fields.Sound, protocolVersion);
                writer.WriteSoundSource(fields.SoundCategory, protocolVersion);
                writer.WriteSignedInt(fields.X);
                writer.WriteSignedInt(fields.Y);
                writer.WriteSignedInt(fields.Z);
                writer.WriteFloat(fields.Volume);
                writer.WriteFloat(fields.Pitch);
                writer.WriteSignedLong(fields.Seed);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SoundEffect), protocolVersion, SupportedVersionsStatic);
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
                    SoundId = reader.ReadVarInt(),
                    SoundCategory = reader.ReadVarInt(),
                    X = reader.ReadSignedInt(),
                    Y = reader.ReadSignedInt(),
                    Z = reader.ReadSignedInt(),
                    Volume = reader.ReadFloat(),
                    Pitch = reader.ReadFloat()
                };
                return;
            case >= 759 and <= 760:
                V759_760 = new V759_760Fields
                {
                    SoundId = reader.ReadVarInt(),
                    SoundCategory = reader.ReadVarInt(),
                    X = reader.ReadSignedInt(),
                    Y = reader.ReadSignedInt(),
                    Z = reader.ReadSignedInt(),
                    Volume = reader.ReadFloat(),
                    Pitch = reader.ReadFloat(),
                    Seed = reader.ReadSignedLong()
                };
                return;
            case >= 761 and <= MinecraftVersion.LatestProtocol:
                V761_Last = new V761_LastFields
                {
                    Sound = reader.ReadItemSoundHolder(protocolVersion),
                    SoundCategory = reader.ReadSoundSource(protocolVersion),
                    X = reader.ReadSignedInt(),
                    Y = reader.ReadSignedInt(),
                    Z = reader.ReadSignedInt(),
                    Volume = reader.ReadFloat(),
                    Pitch = reader.ReadFloat(),
                    Seed = reader.ReadSignedLong()
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SoundEffect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_758Fields
    {
        public int SoundId { get; set; }
        public int SoundCategory { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }
    }

    public struct V759_760Fields
    {
        public int SoundId { get; set; }
        public int SoundCategory { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }
        public long Seed { get; set; }
    }

    public struct V761_LastFields
    {
        public ItemSoundHolder Sound { get; set; }
        public SoundSource SoundCategory { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }
        public long Seed { get; set; }
    }
}
