﻿using McProtoNet.API.IO;
using McProtoNet.API.Networking;

namespace McProtoNet.API
{

    public sealed class LoginStartPacket : IPacket
    {
        public string Nickname { get; set; }
        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Nickname);
        }

        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public LoginStartPacket(string nickname)
        {
            Nickname = nickname;
        }
        public LoginStartPacket()
        {

        }
    }
}
