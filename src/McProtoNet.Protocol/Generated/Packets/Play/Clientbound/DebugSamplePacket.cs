using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toClient.debug_sample", "DebugSample", PacketPhase.Play, PacketDirection.Clientbound, 28);

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
