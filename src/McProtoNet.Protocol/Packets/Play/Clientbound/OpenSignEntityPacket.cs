using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("OpenSignEntity", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class OpenSignEntityPacket : IServerPacket
    {
        public Position Location { get; set; }

        [PacketSubInfo(340, 762)]
        public sealed partial class V340_762 : OpenSignEntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
            }
        }

        [PacketSubInfo(763, 769)]
        public sealed partial class V763_769 : OpenSignEntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                IsFrontText = reader.ReadBoolean();
            }

            public bool IsFrontText { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}