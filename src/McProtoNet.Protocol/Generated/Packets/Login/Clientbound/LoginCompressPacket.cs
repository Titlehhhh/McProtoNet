using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct LoginCompressPacket(int Threshold) : IProtocolType<LoginCompressPacket>
{
    public static LoginCompressPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginCompressPacket>(protocolVersion);
        var threshold = reader.ReadVarInt();
        return new LoginCompressPacket(threshold);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginCompressPacket>(protocolVersion);
        writer.WriteVarInt(Threshold);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 765)
            return 0x03;
        if (protocolVersion >= 766 && protocolVersion <= 772)
            return 0x03;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
