using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("RemoveResourcePack", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract partial class RemoveResourcePackPacket : IServerPacket
{
    [PacketSubInfo(765, 767)]
    public sealed partial class V765_767 : RemoveResourcePackPacket
    {
        public Guid? UUID { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            UUID = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadUUID());
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 765 and <= 767;
        }
    }


    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    public Guid? UUID { get; set; }
}