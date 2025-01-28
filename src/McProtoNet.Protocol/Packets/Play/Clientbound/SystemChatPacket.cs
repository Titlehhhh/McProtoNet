using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SystemChat", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SystemChatPacket : IServerPacket
    {
        [PacketSubInfo(759, 759)]
        public sealed partial class V759 : SystemChatPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Content = reader.ReadString();
                Type = reader.ReadVarInt();
            }

            public string Content { get; set; }
            public int Type { get; set; }
        }

        [PacketSubInfo(760, 764)]
        public sealed partial class V760_764 : SystemChatPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Content = reader.ReadString();
                IsActionBar = reader.ReadBoolean();
            }

            public string Content { get; set; }
            public bool IsActionBar { get; set; }
        }

        [PacketSubInfo(765, 769)]
        public sealed partial class V765_769 : SystemChatPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Content = reader.ReadNbtTag(false);
                IsActionBar = reader.ReadBoolean();
            }

            public NbtTag Content { get; set; }
            public bool IsActionBar { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}