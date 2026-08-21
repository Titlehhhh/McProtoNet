using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public sealed partial class ReportDetail : IProtocolType<ReportDetail>
{
    public string Key { get; }
    public string Value { get; }

    public ReportDetail(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public static ReportDetail Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ReportDetail>(protocolVersion);
        var key = reader.ReadString();
        var value = reader.ReadString();
        return new ReportDetail(key, value);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ReportDetail>(protocolVersion);
        writer.WriteString(Key);
        writer.WriteString(Value);
    }
}
