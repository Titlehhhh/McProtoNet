using System;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WorldParticles", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class WorldParticlesPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, 765),
        new(766, 768),
        new(769, MinecraftVersion.LatestProtocol),
    };

    public bool LongDistance { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public float OffsetX { get; set; }
    public float OffsetY { get; set; }
    public float OffsetZ { get; set; }

    public VFirst_758Fields? VFirst_758 { get; set; }
    public V759_765Fields? V759_765 { get; set; }
    public V766_768Fields? V766_768 { get; set; }
    public V769_LastFields? V769_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                var fields = VFirst_758 ?? throw new InvalidOperationException("WorldParticles VFirst_758 missing.");
                writer.WriteSignedInt(fields.ParticleId);
                writer.WriteBoolean(LongDistance);
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(OffsetX);
                writer.WriteFloat(OffsetY);
                writer.WriteFloat(OffsetZ);
                writer.WriteFloat(fields.ParticleData);
                writer.WriteSignedInt(fields.Particles);
                writer.WriteParticleData(protocolVersion, fields.ParticleId, fields.Data);
                return;
            }
            case >= 759 and <= 765:
            {
                var fields = V759_765 ?? throw new InvalidOperationException("WorldParticles V759_765 missing.");
                writer.WriteVarInt(fields.ParticleId);
                writer.WriteBoolean(LongDistance);
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(OffsetX);
                writer.WriteFloat(OffsetY);
                writer.WriteFloat(OffsetZ);
                writer.WriteFloat(fields.ParticleData);
                writer.WriteSignedInt(fields.Particles);
                writer.WriteParticleData(protocolVersion, fields.ParticleId, fields.Data);
                return;
            }
            case >= 766 and <= 768:
            {
                var fields = V766_768 ?? throw new InvalidOperationException("WorldParticles V766_768 missing.");
                writer.WriteBoolean(LongDistance);
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(OffsetX);
                writer.WriteFloat(OffsetY);
                writer.WriteFloat(OffsetZ);
                writer.WriteFloat(fields.VelocityOffset);
                writer.WriteSignedInt(fields.Amount);
                writer.WriteParticle(fields.Particle, protocolVersion);
                return;
            }
            case >= 769 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V769_Last ?? throw new InvalidOperationException("WorldParticles V769_Last missing.");
                writer.WriteBoolean(LongDistance);
                writer.WriteBoolean(fields.AlwaysShow);
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(OffsetX);
                writer.WriteFloat(OffsetY);
                writer.WriteFloat(OffsetZ);
                writer.WriteFloat(fields.VelocityOffset);
                writer.WriteSignedInt(fields.Amount);
                writer.WriteParticle(fields.Particle, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.WorldParticles), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                int particleId = reader.ReadSignedInt();
                LongDistance = reader.ReadBoolean();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                OffsetX = reader.ReadFloat();
                OffsetY = reader.ReadFloat();
                OffsetZ = reader.ReadFloat();
                VFirst_758 = new VFirst_758Fields
                {
                    ParticleId = particleId,
                    ParticleData = reader.ReadFloat(),
                    Particles = reader.ReadSignedInt(),
                    Data = reader.ReadParticleData(protocolVersion, particleId)
                };
                return;
            }
            case >= 759 and <= 765:
            {
                int particleId = reader.ReadVarInt();
                LongDistance = reader.ReadBoolean();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                OffsetX = reader.ReadFloat();
                OffsetY = reader.ReadFloat();
                OffsetZ = reader.ReadFloat();
                V759_765 = new V759_765Fields
                {
                    ParticleId = particleId,
                    ParticleData = reader.ReadFloat(),
                    Particles = reader.ReadSignedInt(),
                    Data = reader.ReadParticleData(protocolVersion, particleId)
                };
                return;
            }
            case >= 766 and <= 768:
                LongDistance = reader.ReadBoolean();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                OffsetX = reader.ReadFloat();
                OffsetY = reader.ReadFloat();
                OffsetZ = reader.ReadFloat();
                V766_768 = new V766_768Fields
                {
                    VelocityOffset = reader.ReadFloat(),
                    Amount = reader.ReadSignedInt(),
                    Particle = reader.ReadParticle(protocolVersion)
                };
                return;
            case >= 769 and <= MinecraftVersion.LatestProtocol:
            {
                LongDistance = reader.ReadBoolean();
                bool alwaysShow = reader.ReadBoolean();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                OffsetX = reader.ReadFloat();
                OffsetY = reader.ReadFloat();
                OffsetZ = reader.ReadFloat();
                V769_Last = new V769_LastFields
                {
                    AlwaysShow = alwaysShow,
                    VelocityOffset = reader.ReadFloat(),
                    Amount = reader.ReadSignedInt(),
                    Particle = reader.ReadParticle(protocolVersion)
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.WorldParticles), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_758Fields
    {
        public int ParticleId { get; set; }
        public float ParticleData { get; set; }
        public int Particles { get; set; }
        public ParticleData? Data { get; set; }
    }

    public struct V759_765Fields
    {
        public int ParticleId { get; set; }
        public float ParticleData { get; set; }
        public int Particles { get; set; }
        public ParticleData? Data { get; set; }
    }

    public struct V766_768Fields
    {
        public float VelocityOffset { get; set; }
        public int Amount { get; set; }
        public Particle Particle { get; set; }
    }

    public struct V769_LastFields
    {
        public bool AlwaysShow { get; set; }
        public float VelocityOffset { get; set; }
        public int Amount { get; set; }
        public Particle Particle { get; set; }
    }
}
