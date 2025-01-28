using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("WorldBorderWarningDelay", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class WorldBorderWarningDelayPacket : IServerPacket
    {
        public int WarningTime { get; set; }

        [PacketSubInfo(755, 769)]
        public sealed partial class V755_769 : WorldBorderWarningDelayPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WarningTime = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}