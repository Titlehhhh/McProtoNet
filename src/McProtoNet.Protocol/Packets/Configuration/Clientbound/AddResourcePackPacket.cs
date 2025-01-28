using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("AddResourcePack", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract partial class AddResourcePackPacket : IServerPacket
{
    [PacketSubInfo(765, 767)]
    public sealed partial class V765_767 : AddResourcePackPacket
    {
        public Guid Uuid { get; set; }
        public string Url { get; set; }
        public string Hash { get; set; }
        public bool Forced { get; set; }
        public NbtTag? PromptMessage { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Uuid = reader.ReadUUID();
            Url = reader.ReadString();
            Hash = reader.ReadString();
            Forced = reader.ReadBoolean();
            PromptMessage = reader.ReadOptionalNbtTag(readRootTag: false);
        }

    }
    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

}