using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.generate_structure", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Location", "Position")]
[PacketField("Levels", "int")]
[PacketField("KeepJigsaws", "bool")]
public sealed partial record GenerateStructurePacket(Position Location, int Levels, bool KeepJigsaws) : IPacket<GenerateStructurePacket>, IPacket
{
    public static GenerateStructurePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GenerateStructurePacket>(protocolVersion);
        var location = reader.ReadType<Position>(protocolVersion);
        var levels = reader.ReadVarInt();
        var keepJigsaws = reader.ReadBoolean();
        return new GenerateStructurePacket(location, levels, keepJigsaws);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GenerateStructurePacket>(protocolVersion);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteVarInt(Levels);
        writer.WriteBoolean(KeepJigsaws);
    }

    public static PacketIdentity Identity => new("play.toServer.generate_structure", "GenerateStructure", PacketPhase.Play, PacketDirection.Serverbound, 27);

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
