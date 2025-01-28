using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("ChatPreview", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ChatPreviewPacket : IServerPacket
    {
        public int QueryId { get; set; }
        public string? Message { get; set; }

        [PacketSubInfo(759, 760)]
        public sealed partial class V759_760 : ChatPreviewPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                QueryId = reader.ReadSignedInt();
                Message = reader.ReadOptional(ReadDelegates.String);
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}