using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SelectAdvancementTab", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SelectAdvancementTabPacket : IServerPacket
    {
        public string? Id { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : SelectAdvancementTabPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Id = reader.ReadOptional(ReadDelegates.String);
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}