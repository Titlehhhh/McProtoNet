using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x18, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientMoveItemToHotbarPacket : IPacket
    {
        public int Slot { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(Slot);
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            Slot = stream.ReadVarInt();
        }
        public ClientMoveItemToHotbarPacket() { }

        public ClientMoveItemToHotbarPacket(int Slot)
        {
            this.Slot = Slot;
        }
    }
}
