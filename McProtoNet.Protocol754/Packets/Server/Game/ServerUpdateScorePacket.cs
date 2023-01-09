using McProtoNet.Protocol754.Data;
using System;

namespace McProtoNet.Protocol754.Packets.Server
{

    
    public sealed class ServerUpdateScorePacket : Packet 
    {
        public string Entry { get; set; }
        public ScoreboardAction Action { get; set; }
        public string Objective { get; set; }
        public int Value { get; set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Entry = stream.ReadString();
            Action = (ScoreboardAction)stream.ReadVarInt();
            this.Objective = stream.ReadString();
            if (this.Action == ScoreboardAction.ADD_OR_UPDATE)
            {
                this.Value = stream.ReadVarInt();
            }
        }
        public ServerUpdateScorePacket() { }
    }
}

