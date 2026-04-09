using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("MoveMinecart", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class MoveMinecartPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(768, MinecraftVersion.LatestProtocol),
    };

    public int EntityId { get; set; }
    public StepEntry[] Steps { get; set; } = Array.Empty<StepEntry>();

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(EntityId);
                writer.WriteVarInt(Steps.Length);
                for (int i = 0; i < Steps.Length; i++)
                {
                    writer.WriteVec3f(Steps[i].Position, protocolVersion);
                    writer.WriteVec3f(Steps[i].Movement, protocolVersion);
                    writer.WriteFloat(Steps[i].Yaw);
                    writer.WriteFloat(Steps[i].Pitch);
                    writer.WriteFloat(Steps[i].Weight);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.MoveMinecart), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                EntityId = reader.ReadVarInt();
                int count = reader.ReadVarInt();
                if (count == 0)
                {
                    Steps = Array.Empty<StepEntry>();
                    return;
                }

                var steps = new StepEntry[count];
                for (int i = 0; i < steps.Length; i++)
                {
                    steps[i] = new StepEntry
                    {
                        Position = reader.ReadVec3f(protocolVersion),
                        Movement = reader.ReadVec3f(protocolVersion),
                        Yaw = reader.ReadFloat(),
                        Pitch = reader.ReadFloat(),
                        Weight = reader.ReadFloat()
                    };
                }
                Steps = steps;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.MoveMinecart), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct StepEntry
    {
        public Vec3f Position { get; set; }
        public Vec3f Movement { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Weight { get; set; }
    }
}
