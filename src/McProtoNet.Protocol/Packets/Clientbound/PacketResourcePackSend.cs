using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class ResourcePackSendPacket : IServerPacket
    {
        public string Url { get; set; }
        public string Hash { get; set; }

        internal sealed class V340_754 : ResourcePackSendPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Url = reader.ReadString();
                Hash = reader.ReadString();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 754;
            }
        }

        internal sealed class V755_764 : ResourcePackSendPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Url = reader.ReadString();
                Hash = reader.ReadString();
                Forced = reader.ReadBoolean();
                PromptMessage = reader.ReadOptional(ReadDelegates.String);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 755 and <= 764;
            }

            public bool Forced { get; set; }
            public string? PromptMessage { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_754.SupportedVersion(protocolVersion) || V755_764.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.ResourcePackSend;

        public ServerPacket GetPacketId() => PacketId;
    }
}