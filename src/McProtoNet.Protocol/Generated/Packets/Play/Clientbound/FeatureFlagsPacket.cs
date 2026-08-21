using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(761, 763)]
[Packet("play.toClient.feature_flags", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Features", "string[]")]
public sealed partial record FeatureFlagsPacket(string[] Features) : IPacket<FeatureFlagsPacket>, IPacket
{
    public static FeatureFlagsPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FeatureFlagsPacket>(protocolVersion);
        int featuresCount = reader.ReadVarInt();
        var features = new string[featuresCount];
        for (int i = 0; i < features.Length; i++)
            features[i] = reader.ReadString();
        return new FeatureFlagsPacket(features);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FeatureFlagsPacket>(protocolVersion);
        writer.WriteVarInt(Features.Length);
        foreach (var featuresItem in Features)
            writer.WriteString(featuresItem);
    }

    public static PacketIdentity Identity => new("play.toClient.feature_flags", "FeatureFlags", PacketPhase.Play, PacketDirection.Clientbound, 43);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x67;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x6B;
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
