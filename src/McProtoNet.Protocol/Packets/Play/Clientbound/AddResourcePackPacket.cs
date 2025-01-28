using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("AddResourcePack", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class AddResourcePackPacket : IServerPacket
    {
        public Guid Uuid { get; set; }
        public string Url { get; set; }
        public string Hash { get; set; }
        public bool Forced { get; set; }
        public NbtTag? PromptMessage { get; set; }

        [PacketSubInfo(765, 767)]
        public sealed partial class V765_767 : AddResourcePackPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Uuid = reader.ReadUUID();
                Url = reader.ReadString();
                Hash = reader.ReadString();
                Forced = reader.ReadBoolean();
                PromptMessage = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) => r_0.ReadNbtTag(false));
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}