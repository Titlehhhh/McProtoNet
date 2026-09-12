using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.face_player", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("FeetEyes", "int")]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Entity", "FacePlayerEntityTarget?")]
public sealed partial record FacePlayerPacket(int FeetEyes, double X, double Y, double Z, FacePlayerEntityTarget? Entity) : IPacket<FacePlayerPacket>, IPacket
{
    public static FacePlayerPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FacePlayerPacket>(protocolVersion);
        var feetEyes = reader.ReadVarInt();
        var x = reader.ReadDouble();
        var y = reader.ReadDouble();
        var z = reader.ReadDouble();
        FacePlayerEntityTarget? entity = null;
        if (reader.ReadBoolean())
            entity = reader.ReadType<FacePlayerEntityTarget>(protocolVersion);
        return new FacePlayerPacket(feetEyes, x, y, z, entity);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FacePlayerPacket>(protocolVersion);
        writer.WriteVarInt(FeetEyes);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteBoolean(Entity is not null);
        if (Entity is { } entityValue)
            writer.WriteType<FacePlayerEntityTarget>(entityValue, protocolVersion);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("FeetEyes");
        writer.WriteNumberValue(FeetEyes);
        writer.WritePropertyName("X");
        if (double.IsFinite(X))
            writer.WriteNumberValue(X);
        else
            writer.WriteStringValue(double.IsNaN(X) ? "NaN" : X > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Y");
        if (double.IsFinite(Y))
            writer.WriteNumberValue(Y);
        else
            writer.WriteStringValue(double.IsNaN(Y) ? "NaN" : Y > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Z");
        if (double.IsFinite(Z))
            writer.WriteNumberValue(Z);
        else
            writer.WriteStringValue(double.IsNaN(Z) ? "NaN" : Z > 0 ? "Infinity" : "-Infinity");
        if (Entity is { } entityValue)
        {
            writer.WritePropertyName("Entity");
            entityValue.WriteJson(writer);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.face_player", "FacePlayer", PacketPhase.Play, PacketDirection.Clientbound, 45);

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
