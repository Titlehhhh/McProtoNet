using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SetCooldown", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SetCooldownPacket : IServerPacket
    {
        public int CooldownTicks { get; set; }

        [PacketSubInfo(340, 767)]
        public sealed partial class V340_767 : SetCooldownPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ItemID = reader.ReadVarInt();
                CooldownTicks = reader.ReadVarInt();
            }

            public int ItemID { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : SetCooldownPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                CooldownGroup = reader.ReadString();
                CooldownTicks = reader.ReadVarInt();
            }

            public string CooldownGroup { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}