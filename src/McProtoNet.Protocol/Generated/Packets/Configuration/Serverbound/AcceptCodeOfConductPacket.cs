using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[ProtocolSupport(773, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toServer.accept_code_of_conduct", PacketPhase.Configuration, PacketDirection.Serverbound)]
public sealed partial record AcceptCodeOfConductPacket() : IPacket<AcceptCodeOfConductPacket>, IPacket
{
    public static AcceptCodeOfConductPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AcceptCodeOfConductPacket>(protocolVersion);
        return new AcceptCodeOfConductPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AcceptCodeOfConductPacket>(protocolVersion);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("configuration.toServer.accept_code_of_conduct", "AcceptCodeOfConduct", PacketPhase.Configuration, PacketDirection.Serverbound, 0);

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
