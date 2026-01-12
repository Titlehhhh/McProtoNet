using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityEffect", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class EntityEffectPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 757),
        new(758, 758),
        new(759, 763),
        new(764, 765),
        new(766, MinecraftVersion.LatestProtocol),
    };

    public int EntityId { get; set; }
    public int Duration { get; set; }

    public VFirst_757Fields? VFirst_757 { get; set; }
    public V758Fields? V758 { get; set; }
    public V759_763Fields? V759_763 { get; set; }
    public V764_765Fields? V764_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 757:
            {
                var fields = VFirst_757 ?? throw new InvalidOperationException("EntityEffect VFirst_757 missing.");
                writer.WriteVarInt(EntityId);
                writer.WriteSignedByte(fields.EffectId);
                writer.WriteSignedByte(fields.Amplifier);
                writer.WriteVarInt(Duration);
                writer.WriteSignedByte(fields.HideParticles);
                return;
            }
            case 758:
            {
                var fields = V758 ?? throw new InvalidOperationException("EntityEffect V758 missing.");
                writer.WriteVarInt(EntityId);
                writer.WriteVarInt(fields.EffectId);
                writer.WriteSignedByte(fields.Amplifier);
                writer.WriteVarInt(Duration);
                writer.WriteSignedByte(fields.HideParticles);
                return;
            }
            case >= 759 and <= 763:
            {
                var fields = V759_763 ?? throw new InvalidOperationException("EntityEffect V759_763 missing.");
                writer.WriteVarInt(EntityId);
                writer.WriteVarInt(fields.EffectId);
                writer.WriteSignedByte(fields.Amplifier);
                writer.WriteVarInt(Duration);
                writer.WriteSignedByte(fields.HideParticles);
                writer.WriteOptionalNbtTag(fields.FactorCodec, protocolVersion);
                return;
            }
            case >= 764 and <= 765:
            {
                var fields = V764_765 ?? throw new InvalidOperationException("EntityEffect V764_765 missing.");
                writer.WriteVarInt(EntityId);
                writer.WriteVarInt(fields.EffectId);
                writer.WriteSignedByte(fields.Amplifier);
                writer.WriteVarInt(Duration);
                writer.WriteSignedByte(fields.HideParticles);
                writer.WriteAnonOptionalNbtTag(fields.FactorCodec, protocolVersion);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("EntityEffect V766_Last missing.");
                writer.WriteVarInt(EntityId);
                writer.WriteVarInt(fields.EffectId);
                writer.WriteVarInt(fields.Amplifier);
                writer.WriteVarInt(Duration);
                writer.WriteUnsignedByte(fields.Flags);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntityEffect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 757:
            {
                var fields = new VFirst_757Fields();
                EntityId = reader.ReadVarInt();
                fields.EffectId = reader.ReadSignedByte();
                fields.Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                fields.HideParticles = reader.ReadSignedByte();
                VFirst_757 = fields;
                return;
            }
            case 758:
            {
                var fields = new V758Fields();
                EntityId = reader.ReadVarInt();
                fields.EffectId = reader.ReadVarInt();
                fields.Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                fields.HideParticles = reader.ReadSignedByte();
                V758 = fields;
                return;
            }
            case >= 759 and <= 763:
            {
                var fields = new V759_763Fields();
                EntityId = reader.ReadVarInt();
                fields.EffectId = reader.ReadVarInt();
                fields.Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                fields.HideParticles = reader.ReadSignedByte();
                fields.FactorCodec = reader.ReadOptionalNbtTag(protocolVersion);
                V759_763 = fields;
                return;
            }
            case >= 764 and <= 765:
            {
                var fields = new V764_765Fields();
                EntityId = reader.ReadVarInt();
                fields.EffectId = reader.ReadVarInt();
                fields.Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                fields.HideParticles = reader.ReadSignedByte();
                fields.FactorCodec = reader.ReadAnonOptionalNbtTag(protocolVersion);
                V764_765 = fields;
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V766_LastFields();
                EntityId = reader.ReadVarInt();
                fields.EffectId = reader.ReadVarInt();
                fields.Amplifier = reader.ReadVarInt();
                Duration = reader.ReadVarInt();
                fields.Flags = reader.ReadUnsignedByte();
                V766_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntityEffect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_757Fields
    {
        public sbyte EffectId { get; set; }
        public sbyte Amplifier { get; set; }
        public sbyte HideParticles { get; set; }
    }

    public struct V758Fields
    {
        public int EffectId { get; set; }
        public sbyte Amplifier { get; set; }
        public sbyte HideParticles { get; set; }
    }

    public struct V759_763Fields
    {
        public int EffectId { get; set; }
        public sbyte Amplifier { get; set; }
        public sbyte HideParticles { get; set; }
        public NbtTag? FactorCodec { get; set; }
    }

    public struct V764_765Fields
    {
        public int EffectId { get; set; }
        public sbyte Amplifier { get; set; }
        public sbyte HideParticles { get; set; }
        public NbtTag? FactorCodec { get; set; }
    }

    public struct V766_LastFields
    {
        public int EffectId { get; set; }
        public int Amplifier { get; set; }
        public byte Flags { get; set; }
    }
}
