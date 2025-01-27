using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

public abstract class AddResourcePackPacket : IServerPacket
{
    public sealed class V765_767 : AddResourcePackPacket
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

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 765 and <= 767;
        }
    }


    public static bool SupportedVersion(int protocolVersion)
    {
        return V765_767.SupportedVersion(protocolVersion);
    }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    public static PacketIdentifier PacketId => ServerConfigurationPacket.AddResourcePack;

    public PacketIdentifier GetPacketId() => PacketId;
}