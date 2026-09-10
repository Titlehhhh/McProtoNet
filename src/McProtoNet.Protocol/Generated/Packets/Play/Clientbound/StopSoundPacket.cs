using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.stop_sound", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Flags", "int")]
[PacketField("Source", "int?")]
[PacketField("Sound", "string?")]
public sealed partial record StopSoundPacket(int Flags, int? Source, string? Sound) : IPacket<StopSoundPacket>, IPacket
{
    public static StopSoundPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StopSoundPacket>(protocolVersion);
        var flags = reader.ReadSignedByte();
        int? source = default;
        if (flags == 1 || flags == 3)
        {
            var sourceValue = reader.ReadVarInt();
            source = sourceValue;
        }

        string? sound = default;
        if (flags == 2 || flags == 3)
        {
            var soundValue = reader.ReadString();
            sound = soundValue;
        }

        return new StopSoundPacket(flags, source, sound);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StopSoundPacket>(protocolVersion);
        writer.WriteSignedByte((sbyte)Flags);
        if ((sbyte)Flags == 1 || (sbyte)Flags == 3)
        {
            writer.WriteVarInt((Source ?? throw new System.InvalidOperationException("Source is required at this protocol version.")));
        }
        else if (Source is not null)
        {
            throw new System.InvalidOperationException("Source is set, but 'flags' does not select it at this protocol version.");
        }

        if ((sbyte)Flags == 2 || (sbyte)Flags == 3)
        {
            writer.WriteString((Sound ?? throw new System.InvalidOperationException("Sound is required at this protocol version.")));
        }
        else if (Sound is not null)
        {
            throw new System.InvalidOperationException("Sound is set, but 'flags' does not select it at this protocol version.");
        }
    }

    public static PacketIdentity Identity => new("play.toClient.stop_sound", "StopSound", PacketPhase.Play, PacketDirection.Clientbound, 107);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
