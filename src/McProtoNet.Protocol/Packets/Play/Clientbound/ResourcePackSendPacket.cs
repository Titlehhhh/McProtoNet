using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("ResourcePackSend", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ResourcePackSendPacket : IServerPacket
    {
        public string Url { get; set; }
        public string Hash { get; set; }

        [PacketSubInfo(340, 754)]
        public sealed partial class V340_754 : ResourcePackSendPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Url = reader.ReadString();
                Hash = reader.ReadString();
            }
        }

        [PacketSubInfo(755, 764)]
        public sealed partial class V755_764 : ResourcePackSendPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Url = reader.ReadString();
                Hash = reader.ReadString();
                Forced = reader.ReadBoolean();
                PromptMessage = reader.ReadOptional(ReadDelegates.String);
            }

            public bool Forced { get; set; }
            public string? PromptMessage { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}