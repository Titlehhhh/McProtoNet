﻿using ProtoLib.API.IO;
using ProtoLib.API.Networking;

namespace ProtoLib.API
{

    public sealed class LoginDisconnectPacket : IPacket
    {
        public string Message { get; set; }
        public void Read(IMinecraftStreamReader stream)
        {
            Message = stream.ReadString();
        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public LoginDisconnectPacket()
        {

        }

        public LoginDisconnectPacket(string message)
        {
            Message = message;
        }
    }
}
