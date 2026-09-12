using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[Packet("play.toClient.transaction", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("WindowId", "int")]
[PacketField("Action", "int")]
[PacketField("Accepted", "bool")]
public sealed partial record TransactionPacket(int WindowId, int Action, bool Accepted) : IPacket<TransactionPacket>, IPacket
{
    public static TransactionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TransactionPacket>(protocolVersion);
        var windowId = reader.ReadSignedByte();
        var action = reader.ReadSignedShort();
        var accepted = reader.ReadBoolean();
        return new TransactionPacket(windowId, action, accepted);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TransactionPacket>(protocolVersion);
        writer.WriteSignedByte((sbyte)WindowId);
        writer.WriteSignedShort((short)Action);
        writer.WriteBoolean(Accepted);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("WindowId");
        writer.WriteNumberValue(WindowId);
        writer.WritePropertyName("Action");
        writer.WriteNumberValue(Action);
        writer.WritePropertyName("Accepted");
        writer.WriteBooleanValue(Accepted);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.transaction", "Transaction", PacketPhase.Play, PacketDirection.Clientbound, 118);

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
