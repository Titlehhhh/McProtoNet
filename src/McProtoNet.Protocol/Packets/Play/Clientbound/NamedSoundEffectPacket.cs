using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("NamedSoundEffect", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 760)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x19)]
[PacketId(751, 754, 0x18)]
[PacketId(755, 758, 0x19)]
[PacketId(759, 759, 0x16)]
[PacketId(760, 760, 0x17)]
public sealed partial class NamedSoundEffectPacket : IServerPacket
{
    public string SoundName { get; set; }
    public int SoundCategory { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public float Volume { get; set; }
    public float Pitch { get; set; }

    public V759_760Fields? V759_760 { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                writer.WriteString(SoundName);
                writer.WriteVarInt(SoundCategory);
                writer.WriteSignedInt(X);
                writer.WriteSignedInt(Y);
                writer.WriteSignedInt(Z);
                writer.WriteFloat(Volume);
                writer.WriteFloat(Pitch);
                return;

            case >= 759 and <= 760:
            {
                writer.WriteString(SoundName);
                writer.WriteVarInt(SoundCategory);
                writer.WriteSignedInt(X);
                writer.WriteSignedInt(Y);
                writer.WriteSignedInt(Z);
                writer.WriteFloat(Volume);
                writer.WriteFloat(Pitch);
                var fields = V759_760 ?? throw new InvalidOperationException("NamedSoundEffectPacket 759-760 fields missing.");
                writer.WriteSignedLong(fields.Seed);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(NamedSoundEffectPacket), protocolVersion, SupportedVersions);
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
                V759_760 = null;
                return;
            }
            case >= 759 and <= 760:
            {
                SoundName = reader.ReadString();
                SoundCategory = reader.ReadVarInt();
                X = reader.ReadSignedInt();
                Y = reader.ReadSignedInt();
                Z = reader.ReadSignedInt();
                Volume = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                V759_760 = new V759_760Fields { Seed = reader.ReadSignedLong() };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(NamedSoundEffectPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V759_760Fields
    {
        public long Seed { get; set; }
    }
}