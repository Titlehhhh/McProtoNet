using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x27, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientUpdateCommandBlockMinecartPacket : IPacket
    {
        public int EntityId { get; private set; }
        public string Command { get; private set; }
        public bool DoesTrackOutput { get; private set; }

        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteString(Command);
            stream.WriteBoolean(DoesTrackOutput);
        }
        public void Read(IMinecraftStreamReader stream)
        {
            EntityId = stream.ReadVarInt();
            Command = stream.ReadString();
            DoesTrackOutput = stream.ReadBoolean();
        }
        public ClientUpdateCommandBlockMinecartPacket() { }

        public ClientUpdateCommandBlockMinecartPacket(int EntityId, string Command, bool DoesTrackOutput)
        {
            this.EntityId = EntityId;
            this.Command = Command;
            this.DoesTrackOutput = DoesTrackOutput;
        }
    }
}
