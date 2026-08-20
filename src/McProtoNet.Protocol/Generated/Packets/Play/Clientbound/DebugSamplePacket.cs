using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.debug_sample", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Sample", "long[]")]
[PacketField("Type", "int")]
public sealed partial record DebugSamplePacket(long[] Sample, int Type) : IPacket<DebugSamplePacket>, IPacket
{
    public static DebugSamplePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DebugSamplePacket>(protocolVersion);
        int sampleCount = reader.ReadVarInt();
        var sample = new long[sampleCount];
        for (int i = 0; i < sample.Length; i++)
            sample[i] = reader.ReadSignedLong();
        var type = reader.ReadVarInt();
        return new DebugSamplePacket(sample, type);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DebugSamplePacket>(protocolVersion);
        writer.WriteVarInt(Sample.Length);
        foreach (var sampleItem in Sample)
            writer.WriteSignedLong(sampleItem);
        writer.WriteVarInt(Type);
    }

    public static PacketIdentity Identity => new("play.toClient.debug_sample", "DebugSample", PacketPhase.Play, PacketDirection.Clientbound, 26);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 769)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 776)
        {
            id = 0x1E;
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
