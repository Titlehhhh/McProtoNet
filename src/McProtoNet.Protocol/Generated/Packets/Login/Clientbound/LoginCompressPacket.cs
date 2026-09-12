using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("login.toClient.compress", PacketPhase.Login, PacketDirection.Clientbound)]
[PacketField("Threshold", "int")]
public sealed partial record LoginCompressPacket(int Threshold) : IPacket<LoginCompressPacket>, IPacket
{
    public static LoginCompressPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginCompressPacket>(protocolVersion);
        var threshold = reader.ReadVarInt();
        return new LoginCompressPacket(threshold);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginCompressPacket>(protocolVersion);
        writer.WriteVarInt(Threshold);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Threshold");
        writer.WriteNumberValue(Threshold);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("login.toClient.compress", "LoginCompress", PacketPhase.Login, PacketDirection.Clientbound, 0);

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
