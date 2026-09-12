using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.camera", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("CameraId", "int")]
public sealed partial record CameraPacket(int CameraId) : IPacket<CameraPacket>, IPacket
{
    public static CameraPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CameraPacket>(protocolVersion);
        var cameraId = reader.ReadVarInt();
        return new CameraPacket(cameraId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CameraPacket>(protocolVersion);
        writer.WriteVarInt(CameraId);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("CameraId");
        writer.WriteNumberValue(CameraId);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.camera", "Camera", PacketPhase.Play, PacketDirection.Clientbound, 10);

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
