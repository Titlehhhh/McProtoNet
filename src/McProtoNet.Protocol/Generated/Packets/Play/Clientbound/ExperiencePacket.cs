using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.experience", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("ExperienceBar", "float")]
[PacketField("Level", "int")]
[PacketField("TotalExperience", "int")]
public sealed partial record ExperiencePacket(float ExperienceBar, int Level, int TotalExperience) : IPacket<ExperiencePacket>, IPacket
{
    public static ExperiencePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ExperiencePacket>(protocolVersion);
        var experienceBar = reader.ReadFloat();
        var level = reader.ReadVarInt();
        var totalExperience = reader.ReadVarInt();
        return new ExperiencePacket(experienceBar, level, totalExperience);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ExperiencePacket>(protocolVersion);
        writer.WriteFloat(ExperienceBar);
        writer.WriteVarInt(Level);
        writer.WriteVarInt(TotalExperience);
    }

    public static PacketIdentity Identity => new("play.toClient.experience", "Experience", PacketPhase.Play, PacketDirection.Clientbound, 43);

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
