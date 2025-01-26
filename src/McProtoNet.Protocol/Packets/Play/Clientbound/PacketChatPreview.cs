using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class ChatPreviewPacket : IServerPacket
    {
        public int QueryId { get; set; }
        public string? Message { get; set; }

        public sealed class V759_760 : ChatPreviewPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                QueryId = reader.ReadSignedInt();
                Message = reader.ReadOptional(ReadDelegates.String);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 759 and <= 760;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V759_760.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.ChatPreview;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}