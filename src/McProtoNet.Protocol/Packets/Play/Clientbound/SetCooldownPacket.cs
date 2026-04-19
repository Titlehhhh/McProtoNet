using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetCooldown", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x17)]
[PacketId(751, 754, 0x16)]
[PacketId(755, 758, 0x17)]
[PacketId(759, 760, 0x14)]
[PacketId(761, 761, 0x13)]
[PacketId(762, 763, 0x15)]
[PacketId(764, 765, 0x16)]
[PacketId(766, 769, 0x17)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x16)]
public sealed partial class SetCooldownPacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                writer.WriteVarInt(CooldownTicks);
                var fields = VFirst_767 ?? throw new InvalidOperationException("SetCooldownPacket 1-767 fields missing.");
                writer.WriteVarInt(fields.ItemID);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(CooldownTicks);
                var fields = V768_Last ?? throw new InvalidOperationException("SetCooldownPacket 768-last fields missing.");
                writer.WriteString(fields.CooldownGroup);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetCooldownPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                CooldownTicks = reader.ReadVarInt();
                VFirst_767 = new VFirst_767Fields { ItemID = reader.ReadVarInt() };
                V768_Last = null;
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                CooldownTicks = reader.ReadVarInt();
                V768_Last = new V768_LastFields { CooldownGroup = reader.ReadString() };
                VFirst_767 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetCooldownPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public int CooldownTicks { get; set; }

    public VFirst_767Fields? VFirst_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    public struct VFirst_767Fields
    {
        public int ItemID { get; set; }
    }

    public struct V768_LastFields
    {
        public string CooldownGroup { get; set; }
    }
}