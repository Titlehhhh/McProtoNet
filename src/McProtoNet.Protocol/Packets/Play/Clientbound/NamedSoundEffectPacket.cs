using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("NamedSoundEffect", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class NamedSoundEffectPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, 760),
    };

    public string SoundName { get; set; }
    public int SoundCategory { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public float Volume { get; set; }
    public float Pitch { get; set; }

    public V759_760Fields? V759_760 { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                writer.WriteString(SoundName);
                writer.WriteVarInt(SoundCategory);
                writer.WriteSignedInt(X);
                writer.WriteSignedInt(Y);
                writer.WriteSignedInt(Z);
                writer.WriteFloat(Volume);
                writer.WriteFloat(Pitch);
                return;
            }
            case >= 759 and <= 760:
            {
                var fields = V759_760 ?? throw new InvalidOperationException("NamedSoundEffect V759_760 fields missing.");
                writer.WriteString(SoundName);
                writer.WriteVarInt(SoundCategory);
                writer.WriteSignedInt(X);
                writer.WriteSignedInt(Y);
                writer.WriteSignedInt(Z);
                writer.WriteFloat(Volume);
                writer.WriteFloat(Pitch);
                writer.WriteSignedLong(fields.Seed);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.NamedSoundEffect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                SoundName = reader.ReadString();
                SoundCategory = reader.ReadVarInt();
                X = reader.ReadSignedInt();
                Y = reader.ReadSignedInt();
                Z = reader.ReadSignedInt();
                Volume = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                return;
            }
            case >= 759 and <= 760:
            {
                var fields = new V759_760Fields();
                SoundName = reader.ReadString();
                SoundCategory = reader.ReadVarInt();
                X = reader.ReadSignedInt();
                Y = reader.ReadSignedInt();
                Z = reader.ReadSignedInt();
                Volume = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                fields.Seed = reader.ReadSignedLong();
                V759_760 = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.NamedSoundEffect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759_760Fields
    {
        public long Seed { get; set; }
    }

}
