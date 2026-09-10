using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.update_health", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Health", "float")]
[PacketField("Food", "int")]
[PacketField("FoodSaturation", "float")]
public sealed partial record UpdateHealthPacket(float Health, int Food, float FoodSaturation) : IPacket<UpdateHealthPacket>, IPacket
{
    public static UpdateHealthPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateHealthPacket>(protocolVersion);
        var health = reader.ReadFloat();
        var food = reader.ReadVarInt();
        var foodSaturation = reader.ReadFloat();
        return new UpdateHealthPacket(health, food, foodSaturation);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateHealthPacket>(protocolVersion);
        writer.WriteFloat(Health);
        writer.WriteVarInt(Food);
        writer.WriteFloat(FoodSaturation);
    }

    public static PacketIdentity Identity => new("play.toClient.update_health", "UpdateHealth", PacketPhase.Play, PacketDirection.Clientbound, 122);

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
