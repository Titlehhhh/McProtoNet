using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("SelectKnownPacks", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract partial class SelectKnownPacksPacket : IServerPacket
{

    [PacketSubInfo(766, 769)]
    public sealed partial class V766_769 : SelectKnownPacksPacket
    {
        public Packs[] Packs { get; private set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            int count = reader.ReadVarInt();
            Packs[] packs = new Packs[count];
            for (int i = 0; i < count; i++)
            {
                Packs pack = new Packs();
                pack.Namespace = reader.ReadString();
                pack.Id = reader.ReadString();
                pack.Version = reader.ReadString();
                packs[i] = pack;
            }
            Packs = packs;
        }
    }


    public sealed class Packs
    {
        public string Namespace { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
    }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

}