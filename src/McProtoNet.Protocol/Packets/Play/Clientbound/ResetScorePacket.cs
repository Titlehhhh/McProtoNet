using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("ResetScore", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ResetScorePacket : IServerPacket
    {
        public string EntityName { get; set; }
        public string? ObjectiveName { get; set; }

        [PacketSubInfo(765, 769)]
        public sealed partial class V765_769 : ResetScorePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityName = reader.ReadString();
                ObjectiveName = reader.ReadOptional(ReadDelegates.String);
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}