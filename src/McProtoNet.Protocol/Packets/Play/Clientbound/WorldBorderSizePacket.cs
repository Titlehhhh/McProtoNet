using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("WorldBorderSize", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class WorldBorderSizePacket : IServerPacket
    {
        public double Diameter { get; set; }

        [PacketSubInfo(755, 769)]
        public sealed partial class V755_769 : WorldBorderSizePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Diameter = reader.ReadDouble();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}