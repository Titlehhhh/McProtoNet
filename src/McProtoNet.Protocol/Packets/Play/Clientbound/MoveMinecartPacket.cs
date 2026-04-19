using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("MoveMinecart", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[PacketId(768, 769, 0x31)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x30)]
public sealed partial class MoveMinecartPacket : IServerPacket
{
    public int EntityId { get; set; }
    public Step[] Steps { get; set; } = Array.Empty<Step>();

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteVarInt(Steps.Length);
        foreach (var step in Steps)
        {
            writer.WriteType<Position>(step.Position, protocolVersion);
            writer.WriteType<Vec3f>(step.Movement, protocolVersion);
            writer.WriteFloat(step.Yaw);
            writer.WriteFloat(step.Pitch);
            writer.WriteFloat(step.Weight);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        int stepsCount = reader.ReadVarInt();
        var steps = new Step[stepsCount];
        for (int i = 0; i < stepsCount; i++)
        {
            steps[i] = new Step
            {
                Position = reader.ReadType<Position>(protocolVersion),
                Movement = reader.ReadType<Vec3f>(protocolVersion),
                Yaw = reader.ReadFloat(),
                Pitch = reader.ReadFloat(),
                Weight = reader.ReadFloat()
            };
        }
        Steps = steps;
    }

    public struct Step
    {
        public Position Position { get; set; }
        public Vec3f Movement { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Weight { get; set; }
    }
}