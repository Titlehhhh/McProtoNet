using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntitySoundEffect", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class EntitySoundEffectPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, 760),
        new(761, 765),
        new(766, MinecraftVersion.LatestProtocol),
    };

    public VFirst_758Fields? VFirst_758 { get; set; }
    public V759_760Fields? V759_760 { get; set; }
    public V761_765Fields? V761_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                var fields = VFirst_758 ?? throw new InvalidOperationException("EntitySoundEffect VFirst_758 fields missing.");
                writer.WriteVarInt(fields.SoundId);
                writer.WriteVarInt(fields.SoundCategory);
                writer.WriteVarInt(fields.EntityId);
                writer.WriteFloat(fields.Volume);
                writer.WriteFloat(fields.Pitch);
                return;
            }
            case >= 759 and <= 760:
            {
                var fields = V759_760 ?? throw new InvalidOperationException("EntitySoundEffect V759_760 fields missing.");
                writer.WriteVarInt(fields.SoundId);
                writer.WriteVarInt(fields.SoundCategory);
                writer.WriteVarInt(fields.EntityId);
                writer.WriteFloat(fields.Volume);
                writer.WriteFloat(fields.Pitch);
                writer.WriteSignedLong(fields.Seed);
                return;
            }
            case >= 761 and <= 765:
            {
                var fields = V761_765 ?? throw new InvalidOperationException("EntitySoundEffect V761_765 fields missing.");
                writer.WriteVarInt(fields.SoundId);
                if (fields.SoundId == 0)
                {
                    var soundEvent = fields.SoundEvent ?? throw new InvalidOperationException("EntitySoundEffect sound event missing.");
                    writer.WriteString(soundEvent.Resource);
                    if (soundEvent.Range.HasValue)
                    {
                        writer.WriteBoolean(true);
                        writer.WriteFloat(soundEvent.Range.Value);
                    }
                    else
                    {
                        writer.WriteBoolean(false);
                    }
                }
                writer.WriteSoundSource(fields.SoundCategory, protocolVersion);
                writer.WriteVarInt(fields.EntityId);
                writer.WriteFloat(fields.Volume);
                writer.WriteFloat(fields.Pitch);
                writer.WriteSignedLong(fields.Seed);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("EntitySoundEffect V766_Last fields missing.");
                writer.WriteItemSoundHolder(fields.Sound, protocolVersion);
                writer.WriteSoundSource(fields.SoundCategory, protocolVersion);
                writer.WriteVarInt(fields.EntityId);
                writer.WriteFloat(fields.Volume);
                writer.WriteFloat(fields.Pitch);
                writer.WriteSignedLong(fields.Seed);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntitySoundEffect), protocolVersion, SupportedVersionsStatic);
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
                    EntityId = reader.ReadVarInt(),
                    Volume = reader.ReadFloat(),
                    Pitch = reader.ReadFloat()
                };
                return;
            case >= 759 and <= 760:
                V759_760 = new V759_760Fields
                {
                    SoundId = reader.ReadVarInt(),
                    SoundCategory = reader.ReadVarInt(),
                    EntityId = reader.ReadVarInt(),
                    Volume = reader.ReadFloat(),
                    Pitch = reader.ReadFloat(),
                    Seed = reader.ReadSignedLong()
                };
                return;
            case >= 761 and <= 765:
            {
                var fields = new V761_765Fields
                {
                    SoundId = reader.ReadVarInt()
                };
                if (fields.SoundId == 0)
                {
                    fields.SoundEvent = new SoundEvent
                    {
                        Resource = reader.ReadString(),
                        Range = reader.ReadOptional(ReadDelegates.Float)
                    };
                }
                fields.SoundCategory = reader.ReadSoundSource(protocolVersion);
                fields.EntityId = reader.ReadVarInt();
                fields.Volume = reader.ReadFloat();
                fields.Pitch = reader.ReadFloat();
                fields.Seed = reader.ReadSignedLong();
                V761_765 = fields;
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                V766_Last = new V766_LastFields
                {
                    Sound = reader.ReadItemSoundHolder(protocolVersion),
                    SoundCategory = reader.ReadSoundSource(protocolVersion),
                    EntityId = reader.ReadVarInt(),
                    Volume = reader.ReadFloat(),
                    Pitch = reader.ReadFloat(),
                    Seed = reader.ReadSignedLong()
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntitySoundEffect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_758Fields
    {
        public int SoundId { get; set; }
        public int SoundCategory { get; set; }
        public int EntityId { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }
    }

    public struct V759_760Fields
    {
        public int SoundId { get; set; }
        public int SoundCategory { get; set; }
        public int EntityId { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }
        public long Seed { get; set; }
    }

    public struct V761_765Fields
    {
        public int SoundId { get; set; }
        public SoundEvent? SoundEvent { get; set; }
        public SoundSource SoundCategory { get; set; }
        public int EntityId { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }
        public long Seed { get; set; }
    }

    public struct V766_LastFields
    {
        public ItemSoundHolder Sound { get; set; }
        public SoundSource SoundCategory { get; set; }
        public int EntityId { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }
        public long Seed { get; set; }
    }

    public struct SoundEvent
    {
        public string Resource { get; set; }
        public float? Range { get; set; }
    }
}
