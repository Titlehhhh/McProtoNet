using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Explosion", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ExplosionPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
        new(755, 760),
        new(761, 764),
        new(765, 767),
        new(768, 768),
        new(769, MinecraftVersion.LatestProtocol),
    };

    public VFirst_754Fields? VFirst_754 { get; set; }
    public V755_760Fields? V755_760 { get; set; }
    public V761_764Fields? V761_764 { get; set; }
    public V765_767Fields? V765_767 { get; set; }
    public V768Fields? V768 { get; set; }
    public V769_LastFields? V769_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                var fields = VFirst_754 ?? throw new InvalidOperationException("Explosion VFirst_754 fields missing.");
                writer.WriteFloat(fields.X);
                writer.WriteFloat(fields.Y);
                writer.WriteFloat(fields.Z);
                writer.WriteFloat(fields.Radius);
                writer.WriteSignedInt(fields.AffectedBlockOffsets.Length);
                for (int i = 0; i < fields.AffectedBlockOffsets.Length; i++)
                {
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].X);
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].Y);
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].Z);
                }
                writer.WriteFloat(fields.PlayerMotionX);
                writer.WriteFloat(fields.PlayerMotionY);
                writer.WriteFloat(fields.PlayerMotionZ);
                return;
            }
            case >= 755 and <= 760:
            {
                var fields = V755_760 ?? throw new InvalidOperationException("Explosion V755_760 fields missing.");
                writer.WriteFloat(fields.X);
                writer.WriteFloat(fields.Y);
                writer.WriteFloat(fields.Z);
                writer.WriteFloat(fields.Radius);
                writer.WriteVarInt(fields.AffectedBlockOffsets.Length);
                for (int i = 0; i < fields.AffectedBlockOffsets.Length; i++)
                {
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].X);
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].Y);
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].Z);
                }
                writer.WriteFloat(fields.PlayerMotionX);
                writer.WriteFloat(fields.PlayerMotionY);
                writer.WriteFloat(fields.PlayerMotionZ);
                return;
            }
            case >= 761 and <= 764:
            {
                var fields = V761_764 ?? throw new InvalidOperationException("Explosion V761_764 fields missing.");
                writer.WriteDouble(fields.X);
                writer.WriteDouble(fields.Y);
                writer.WriteDouble(fields.Z);
                writer.WriteFloat(fields.Radius);
                writer.WriteVarInt(fields.AffectedBlockOffsets.Length);
                for (int i = 0; i < fields.AffectedBlockOffsets.Length; i++)
                {
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].X);
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].Y);
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].Z);
                }
                writer.WriteFloat(fields.PlayerMotionX);
                writer.WriteFloat(fields.PlayerMotionY);
                writer.WriteFloat(fields.PlayerMotionZ);
                return;
            }
            case >= 765 and <= 767:
            {
                var fields = V765_767 ?? throw new InvalidOperationException("Explosion V765_767 fields missing.");
                writer.WriteDouble(fields.X);
                writer.WriteDouble(fields.Y);
                writer.WriteDouble(fields.Z);
                writer.WriteFloat(fields.Radius);
                writer.WriteVarInt(fields.AffectedBlockOffsets.Length);
                for (int i = 0; i < fields.AffectedBlockOffsets.Length; i++)
                {
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].X);
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].Y);
                    writer.WriteSignedByte(fields.AffectedBlockOffsets[i].Z);
                }
                writer.WriteFloat(fields.PlayerMotionX);
                writer.WriteFloat(fields.PlayerMotionY);
                writer.WriteFloat(fields.PlayerMotionZ);
                writer.WriteVarInt(fields.BlockInteractionType);
                writer.WriteParticle(fields.SmallExplosionParticle, protocolVersion);
                writer.WriteParticle(fields.LargeExplosionParticle, protocolVersion);
                writer.WriteItemSoundHolder(fields.Sound, protocolVersion);
                return;
            }
            case 768:
            {
                var fields = V768 ?? throw new InvalidOperationException("Explosion V768 fields missing.");
                writer.WriteDouble(fields.X);
                writer.WriteDouble(fields.Y);
                writer.WriteDouble(fields.Z);
                if (fields.PlayerKnockback is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteVec3f(fields.PlayerKnockback.Value, protocolVersion);
                }
                writer.WriteParticle(fields.ExplosionParticle, protocolVersion);
                writer.WriteItemSoundHolder(fields.Sound, protocolVersion);
                return;
            }
            case >= 769 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V769_Last ?? throw new InvalidOperationException("Explosion V769_Last fields missing.");
                writer.WriteDouble(fields.X);
                writer.WriteDouble(fields.Y);
                writer.WriteDouble(fields.Z);
                if (fields.PlayerKnockback is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteVec3f64(fields.PlayerKnockback.Value, protocolVersion);
                }
                writer.WriteParticle(fields.ExplosionParticle, protocolVersion);
                writer.WriteItemSoundHolder(fields.Sound, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Explosion), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                VFirst_754 = new VFirst_754Fields
                {
                    X = reader.ReadFloat(),
                    Y = reader.ReadFloat(),
                    Z = reader.ReadFloat(),
                    Radius = reader.ReadFloat(),
                    AffectedBlockOffsets = ReadOffsets(ref reader, reader.ReadSignedInt()),
                    PlayerMotionX = reader.ReadFloat(),
                    PlayerMotionY = reader.ReadFloat(),
                    PlayerMotionZ = reader.ReadFloat()
                };
                return;
            }
            case >= 755 and <= 760:
            {
                V755_760 = new V755_760Fields
                {
                    X = reader.ReadFloat(),
                    Y = reader.ReadFloat(),
                    Z = reader.ReadFloat(),
                    Radius = reader.ReadFloat(),
                    AffectedBlockOffsets = ReadOffsets(ref reader, reader.ReadVarInt()),
                    PlayerMotionX = reader.ReadFloat(),
                    PlayerMotionY = reader.ReadFloat(),
                    PlayerMotionZ = reader.ReadFloat()
                };
                return;
            }
            case >= 761 and <= 764:
            {
                V761_764 = new V761_764Fields
                {
                    X = reader.ReadDouble(),
                    Y = reader.ReadDouble(),
                    Z = reader.ReadDouble(),
                    Radius = reader.ReadFloat(),
                    AffectedBlockOffsets = ReadOffsets(ref reader, reader.ReadVarInt()),
                    PlayerMotionX = reader.ReadFloat(),
                    PlayerMotionY = reader.ReadFloat(),
                    PlayerMotionZ = reader.ReadFloat()
                };
                return;
            }
            case >= 765 and <= 767:
            {
                V765_767 = new V765_767Fields
                {
                    X = reader.ReadDouble(),
                    Y = reader.ReadDouble(),
                    Z = reader.ReadDouble(),
                    Radius = reader.ReadFloat(),
                    AffectedBlockOffsets = ReadOffsets(ref reader, reader.ReadVarInt()),
                    PlayerMotionX = reader.ReadFloat(),
                    PlayerMotionY = reader.ReadFloat(),
                    PlayerMotionZ = reader.ReadFloat(),
                    BlockInteractionType = reader.ReadVarInt(),
                    SmallExplosionParticle = reader.ReadParticle(protocolVersion),
                    LargeExplosionParticle = reader.ReadParticle(protocolVersion),
                    Sound = reader.ReadItemSoundHolder(protocolVersion)
                };
                return;
            }
            case 768:
            {
                V768 = new V768Fields
                {
                    X = reader.ReadDouble(),
                    Y = reader.ReadDouble(),
                    Z = reader.ReadDouble(),
                    PlayerKnockback = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadVec3f(protocolVersion)),
                    ExplosionParticle = reader.ReadParticle(protocolVersion),
                    Sound = reader.ReadItemSoundHolder(protocolVersion)
                };
                return;
            }
            case >= 769 and <= MinecraftVersion.LatestProtocol:
            {
                V769_Last = new V769_LastFields
                {
                    X = reader.ReadDouble(),
                    Y = reader.ReadDouble(),
                    Z = reader.ReadDouble(),
                    PlayerKnockback = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadVec3f64(protocolVersion)),
                    ExplosionParticle = reader.ReadParticle(protocolVersion),
                    Sound = reader.ReadItemSoundHolder(protocolVersion)
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Explosion), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static BlockOffset[] ReadOffsets(ref MinecraftPrimitiveReader reader, int count)
    {
        if (count == 0)
        {
            return Array.Empty<BlockOffset>();
        }

        var offsets = new BlockOffset[count];
        for (int i = 0; i < offsets.Length; i++)
        {
            offsets[i] = new BlockOffset
            {
                X = reader.ReadSignedByte(),
                Y = reader.ReadSignedByte(),
                Z = reader.ReadSignedByte()
            };
        }
        return offsets;
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_754Fields
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Radius { get; set; }
        public BlockOffset[] AffectedBlockOffsets { get; set; }
        public float PlayerMotionX { get; set; }
        public float PlayerMotionY { get; set; }
        public float PlayerMotionZ { get; set; }
    }

    public struct V755_760Fields
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Radius { get; set; }
        public BlockOffset[] AffectedBlockOffsets { get; set; }
        public float PlayerMotionX { get; set; }
        public float PlayerMotionY { get; set; }
        public float PlayerMotionZ { get; set; }
    }

    public struct V761_764Fields
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Radius { get; set; }
        public BlockOffset[] AffectedBlockOffsets { get; set; }
        public float PlayerMotionX { get; set; }
        public float PlayerMotionY { get; set; }
        public float PlayerMotionZ { get; set; }
    }

    public struct V765_767Fields
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Radius { get; set; }
        public BlockOffset[] AffectedBlockOffsets { get; set; }
        public float PlayerMotionX { get; set; }
        public float PlayerMotionY { get; set; }
        public float PlayerMotionZ { get; set; }
        public int BlockInteractionType { get; set; }
        public Particle SmallExplosionParticle { get; set; }
        public Particle LargeExplosionParticle { get; set; }
        public ItemSoundHolder Sound { get; set; }
    }

    public struct V768Fields
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Vec3f? PlayerKnockback { get; set; }
        public Particle ExplosionParticle { get; set; }
        public ItemSoundHolder Sound { get; set; }
    }

    public struct V769_LastFields
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Vec3f64? PlayerKnockback { get; set; }
        public Particle ExplosionParticle { get; set; }
        public ItemSoundHolder Sound { get; set; }
    }

    public struct BlockOffset
    {
        public sbyte X { get; set; }
        public sbyte Y { get; set; }
        public sbyte Z { get; set; }
    }
}
