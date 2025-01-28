using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("ActionBar", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ActionBarPacket : IServerPacket
    {
        [PacketSubInfo(755, 764)]
        public sealed partial class V755_764 : ActionBarPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Text = reader.ReadString();
            }

            public string Text { get; set; }
        }

        [PacketSubInfo(765, 769)]
        public sealed partial class V765_769 : ActionBarPacket
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