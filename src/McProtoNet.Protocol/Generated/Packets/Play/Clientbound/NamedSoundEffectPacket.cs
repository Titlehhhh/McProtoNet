using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 760)]
[Packet("play.toClient.named_sound_effect", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("SoundName", "string")]
[PacketField("SoundCategory", "int")]
[PacketField("X", "int")]
[PacketField("Y", "int")]
[PacketField("Z", "int")]
[PacketField("Volume", "float")]
[PacketField("Pitch", "float")]
[PacketField("Seed", "long", Group = "V759_760", From = 759, To = 760)]
public sealed partial record NamedSoundEffectPacket(string SoundName, int SoundCategory, int X, int Y, int Z, float Volume, float Pitch, NamedSoundEffectPacket.V759_760Layer? V759_760 = null) : IPacket<NamedSoundEffectPacket>, IPacket
{
    public readonly record struct V759_760Layer(long Seed);
    public static NamedSoundEffectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<NamedSoundEffectPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var soundName = reader.ReadString();
            var soundCategory = reader.ReadVarInt();
            var x = reader.ReadSignedInt();
            var y = reader.ReadSignedInt();
            var z = reader.ReadSignedInt();
            var volume = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            return new NamedSoundEffectPacket(soundName, soundCategory, x, y, z, volume, pitch);
        }

        if (protocolVersion >= 759 && protocolVersion <= 760)
        {
            var soundName = reader.ReadString();
            var soundCategory = reader.ReadVarInt();
            var x = reader.ReadSignedInt();
            var y = reader.ReadSignedInt();
            var z = reader.ReadSignedInt();
            var volume = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var seed = reader.ReadSignedLong();
            return new NamedSoundEffectPacket(soundName, soundCategory, x, y, z, volume, pitch, V759_760: new V759_760Layer(seed));
        }

        throw new System.NotSupportedException($"NamedSoundEffectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<NamedSoundEffectPacket>(protocolVersion);
        if (protocolVersion <= 758)
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

        if (protocolVersion >= 759 && protocolVersion <= 760)
        {
            var layer = V759_760 ?? throw new WrongLayerException("NamedSoundEffectPacket", protocolVersion, "V759_760");
            long Seed = layer.Seed;
            writer.WriteString(SoundName);
            writer.WriteVarInt(SoundCategory);
            writer.WriteSignedInt(X);
            writer.WriteSignedInt(Y);
            writer.WriteSignedInt(Z);
            writer.WriteFloat(Volume);
            writer.WriteFloat(Pitch);
            writer.WriteSignedLong(Seed);
            return;
        }

        throw new System.NotSupportedException($"NamedSoundEffectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.named_sound_effect", "NamedSoundEffect", PacketPhase.Play, PacketDirection.Clientbound, 58);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x16;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x17;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
