using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("WorldEvent", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class WorldEventPacket : IServerPacket
    {
        public int EffectId { get; set; }
        public Position Location { get; set; }
        public int Data { get; set; }
        public bool Global { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : WorldEventPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EffectId = reader.ReadSignedInt();
                Location = reader.ReadPosition(protocolVersion);
                Data = reader.ReadSignedInt();
                Global = reader.ReadBoolean();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}