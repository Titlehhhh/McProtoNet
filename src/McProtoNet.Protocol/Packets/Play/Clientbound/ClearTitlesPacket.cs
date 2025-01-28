using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("ClearTitles", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ClearTitlesPacket : IServerPacket
    {
        public bool Reset { get; set; }

        [PacketSubInfo(755, 769)]
        public sealed partial class V755_769 : ClearTitlesPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Reset = reader.ReadBoolean();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}