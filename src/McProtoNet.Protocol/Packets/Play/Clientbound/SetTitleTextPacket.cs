using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SetTitleText", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SetTitleTextPacket : IServerPacket
    {
        [PacketSubInfo(755, 764)]
        public sealed partial class V755_764 : SetTitleTextPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Text = reader.ReadString();
            }

            public string Text { get; set; }
        }

        [PacketSubInfo(765, 769)]
        public sealed partial class V765_769 : SetTitleTextPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Text = reader.ReadNbtTag(false);
            }

            public NbtTag Text { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}