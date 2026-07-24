using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class KnownPack : IProtocolType<KnownPack>
{
    public string Namespace { get; }
    public string Id { get; }
    public string Version { get; }

    public KnownPack(string @namespace, string id, string version)
    {
        Namespace = @namespace;
        Id = id;
        Version = version;
    }

    public static KnownPack Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<KnownPack>(protocolVersion);
        var @namespace = reader.ReadString();
        var id = reader.ReadString();
        var version = reader.ReadString();
        return new KnownPack(@namespace, id, version);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<KnownPack>(protocolVersion);
        writer.WriteString(Namespace);
        writer.WriteString(Id);
        writer.WriteString(Version);
    }
}
