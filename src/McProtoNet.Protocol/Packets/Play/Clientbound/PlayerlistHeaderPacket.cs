using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("PlayerlistHeader", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class PlayerlistHeaderPacket : IServerPacket
    {
        [PacketSubInfo(340, 764)]
        public sealed partial class V340_764 : PlayerlistHeaderPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Header = reader.ReadString();
                Footer = reader.ReadString();
            }

            public string Header { get; set; }
            public string Footer { get; set; }
        }

        [PacketSubInfo(765, 769)]
        public sealed partial class V765_769 : PlayerlistHeaderPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Header = reader.ReadNbtTag(false);
                Footer = reader.ReadNbtTag(false);
            }

            public NbtTag Header { get; set; }
            public NbtTag Footer { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}