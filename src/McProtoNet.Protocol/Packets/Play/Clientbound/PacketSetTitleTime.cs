using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class SetTitleTimePacket : IServerPacket
    {
        public int FadeIn { get; set; }
        public int Stay { get; set; }
        public int FadeOut { get; set; }

        public sealed class V755_769 : SetTitleTimePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                FadeIn = reader.ReadSignedInt();
                Stay = reader.ReadSignedInt();
                FadeOut = reader.ReadSignedInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 755 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V755_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.SetTitleTime;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}