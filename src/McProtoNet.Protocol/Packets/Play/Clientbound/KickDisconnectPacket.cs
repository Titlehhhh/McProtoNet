using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("KickDisconnect", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class KickDisconnectPacket : IServerPacket
    {
        [PacketSubInfo(340, 764)]
        public sealed partial class V340_764 : KickDisconnectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Reason = reader.ReadString();
            }

            public string Reason { get; set; }
        }

        [PacketSubInfo(765, 769)]
        public sealed partial class V765_769 : KickDisconnectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Reason = reader.ReadNbtTag(false);
            }

            public NbtTag Reason { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}