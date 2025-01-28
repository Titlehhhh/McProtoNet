using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SetTitleTime", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SetTitleTimePacket : IServerPacket
    {
        public int FadeIn { get; set; }
        public int Stay { get; set; }
        public int FadeOut { get; set; }

        [PacketSubInfo(755, 769)]
        public sealed partial class V755_769 : SetTitleTimePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                FadeIn = reader.ReadSignedInt();
                Stay = reader.ReadSignedInt();
                FadeOut = reader.ReadSignedInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}