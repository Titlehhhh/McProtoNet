using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("ShouldDisplayChatPreview", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ShouldDisplayChatPreviewPacket : IServerPacket
    {
        public bool ShouldDisplayChatPreview { get; set; }

        [PacketSubInfo(759, 760)]
        public sealed partial class V759_760 : ShouldDisplayChatPreviewPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ShouldDisplayChatPreview = reader.ReadBoolean();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}