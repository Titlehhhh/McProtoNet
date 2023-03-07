﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.Protocol756.Packets.Client
{
    public sealed class EncryptionResponsePacket : MinecraftPacket
    {
        public byte[] SharedKey { get; private set; }
        public byte[] VerifyToken { get; private set; }

        public EncryptionResponsePacket(byte[] sharedKey, byte[] verifyToken)
        {
            SharedKey = sharedKey;
            VerifyToken = verifyToken;
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteByteArray(SharedKey);
            stream.WriteByteArray(VerifyToken);
        }

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            SharedKey = stream.ReadByteArray();
            VerifyToken = stream.ReadByteArray();
        }
        public EncryptionResponsePacket()
        {

        }

    }
}
