using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.feature_flags", PacketPhase.Configuration, PacketDirection.Clientbound)]
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

    public static PacketIdentity Identity => new("configuration.toClient.feature_flags", "FeatureFlags", PacketPhase.Configuration, PacketDirection.Clientbound, 7);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 776)
        {
            id = 0x0C;
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
