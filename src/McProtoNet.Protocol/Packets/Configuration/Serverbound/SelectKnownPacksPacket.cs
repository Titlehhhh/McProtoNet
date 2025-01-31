using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("SelectKnownPacks", PacketState.Configuration, PacketDirection.Serverbound)]
public partial class SelectKnownPacksPacket : IClientPacket
{

    [PacketSubInfo(766, 769)]
    public sealed partial class V766_769 : SelectKnownPacksPacket
    {
        public Packs[] Packs { get; set; }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Packs[] packs)
        {
            writer.WriteVarInt(packs.Length);
            foreach (var pack in packs)
            {
                writer.WriteString(pack.Namespace);
                writer.WriteString(pack.Id);
                writer.WriteString(pack.Version);
            }

        }

        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Packs);
        }
    }
    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V766_769.IsSupportedVersionStatic(protocolVersion))
        {
            V766_769.SerializeInternal(ref writer, protocolVersion, []);
        }else{
            throw new ProtocolNotSupportException(nameof(SelectKnownPacksPacket), protocolVersion);
        }
    }

    public sealed class Packs
    {
        public string Namespace { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }



    }
}