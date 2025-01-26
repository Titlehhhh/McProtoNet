using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class SyncEntityPositionPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Dx { get; set; }
        public double Dy { get; set; }
        public double Dz { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }

        public sealed class V768_769 : SyncEntityPositionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Dx = reader.ReadDouble();
                Dy = reader.ReadDouble();
                Dz = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                OnGround = reader.ReadBoolean();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V768_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.SyncEntityPosition;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}