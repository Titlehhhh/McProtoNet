using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("Disconnect", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x01)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x02)]
public sealed partial class DisconnectPacket : IServerPacket
{
    public VFirst_764Fields? VFirst_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = VFirst_764 ?? throw new InvalidOperationException("DisconnectPacket 764 fields missing.");
                writer.WriteString(fields.Reason);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("DisconnectPacket 765-last fields missing.");
                writer.WriteAnonymousNbtTag(fields.Reason, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(DisconnectPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                VFirst_764 = new VFirst_764Fields { Reason = reader.ReadString() };
                V765_Last = null;
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                V765_Last = new V765_LastFields { Reason = reader.ReadAnonymousNbtTag(protocolVersion) };
                VFirst_764 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(DisconnectPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct VFirst_764Fields
    {
        public string Reason { get; set; }
    }

    public struct V765_LastFields
    {
        public NbtTag Reason { get; set; }
    }
}