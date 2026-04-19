using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SystemChat", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(759, MinecraftVersion.LatestProtocol)]
[PacketId(759, 759, 0x5F)]
[PacketId(760, 760, 0x62)]
[PacketId(761, 761, 0x60)]
[PacketId(762, 763, 0x64)]
[PacketId(764, 764, 0x67)]
[PacketId(765, 765, 0x69)]
[PacketId(766, 767, 0x6C)]
[PacketId(768, 769, 0x73)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x72)]
public sealed partial class SystemChatPacket : IServerPacket
{
    public bool? IsActionBar { get; set; }

    public V759_759Fields? V759_759 { get; set; }
    public V760_764Fields? V760_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 759:
            {
                var fields = V759_759 ?? throw new InvalidOperationException("SystemChatPacket 759 fields missing.");
                writer.WriteString(fields.Content);
                writer.WriteVarInt(fields.Type);
                return;
            }
            case >= 760 and <= 764:
            {
                var fields = V760_764 ?? throw new InvalidOperationException("SystemChatPacket 760-764 fields missing.");
                writer.WriteString(fields.Content);
                writer.WriteBoolean(IsActionBar.Value);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("SystemChatPacket 765-last fields missing.");
                writer.WriteAnonymousNbtTag(fields.Content, protocolVersion);
                writer.WriteBoolean(IsActionBar.Value);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SystemChatPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 759:
            {
                V759_759 = new V759_759Fields
                {
                    Content = reader.ReadString(),
                    Type = reader.ReadVarInt()
                };
                return;
            }
            case >= 760 and <= 764:
            {
                V760_764 = new V760_764Fields
                {
                    Content = reader.ReadString()
                };
                IsActionBar = reader.ReadBoolean();
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                V765_Last = new V765_LastFields
                {
                    Content = reader.ReadAnonymousNbtTag(protocolVersion)
                };
                IsActionBar = reader.ReadBoolean();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SystemChatPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V759_759Fields
    {
        public string Content { get; set; }
        public int Type { get; set; }
    }

    public struct V760_764Fields
    {
        public string Content { get; set; }
    }

    public struct V765_LastFields
    {
        public NbtTag Content { get; set; }
    }
}