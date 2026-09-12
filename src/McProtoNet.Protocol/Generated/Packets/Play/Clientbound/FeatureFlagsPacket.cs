using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Features");
        writer.WriteStartArray();
        foreach (var item0 in Features)
        {
            writer.WriteStringValue(item0);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.feature_flags", "FeatureFlags", PacketPhase.Play, PacketDirection.Clientbound, 46);

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
