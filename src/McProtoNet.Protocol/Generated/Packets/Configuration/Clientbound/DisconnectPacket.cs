using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
public sealed partial class DisconnectPacket : IProtocolType<DisconnectPacket>
{
    public string ReasonJson { get; }
    public NbtTag Reason { get; }

    public DisconnectPacket(string reasonJson, NbtTag reason)
    {
        ReasonJson = reasonJson;
        Reason = reason;
    }

    public static DisconnectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DisconnectPacket>(protocolVersion);
        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            var reasonJson = reader.ReadString();
            return new DisconnectPacket(reasonJson, default!);
        }

        if (protocolVersion >= 765)
        {
            var reason = reader.ReadNbtTag(true)!;
            return new DisconnectPacket(default!, reason);
        }

        throw new System.NotSupportedException($"DisconnectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DisconnectPacket>(protocolVersion);
        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            writer.WriteString(ReasonJson);
            return;
        }

        if (protocolVersion >= 765)
        {
            writer.WriteNbt(Reason);
            return;
        }

        throw new System.NotSupportedException($"DisconnectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x01;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x01;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x02;
        if (protocolVersion >= 767 && protocolVersion <= 770)
            return 0x02;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x02;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
