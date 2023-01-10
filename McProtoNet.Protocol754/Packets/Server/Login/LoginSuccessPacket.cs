﻿namespace McProtoNet.Protocol754.Packets.Server
{
    public sealed class LoginSuccessPacket : MinecraftPacket<Protocol754>
    {
        public Guid UUID { get; set; }
        public string Username { get; set; }

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            UUID = stream.ReadUUID();
            Username = stream.ReadString();
        }



        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(UUID.ToString());
            stream.WriteString(Username);
        }

        public LoginSuccessPacket(Guid uUID, string username)
        {
            UUID = uUID;
            Username = username;
        }

        public LoginSuccessPacket()
        {

        }
    }
}
