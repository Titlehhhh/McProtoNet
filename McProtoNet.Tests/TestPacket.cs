
using McProtoNet.Core.IO;
using McProtoNet.Core.Packets;
using McProtoNet.Core.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace McProtoNet.Tests
{
    public class TestPacket : MinecraftPacket
    {
        public byte[] VeryData { get; set; } = new byte[1000];

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            VeryData = stream.ReadByteArray();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            //  Trace.WriteLine("ms.pos: " + ms.Position);
            //stream.WriteVarInt(VeryData.Length);
            stream.WriteByteArray(VeryData);
            //  Trace.WriteLine("ms.pos: " + ms.Position);
            //stream.Write(VeryData);
            //  Trace.WriteLine("ms.pos: " + ms.Position);
        }
        public override bool Equals(object? obj)
        {
            if (obj is TestPacket packet)
            {
                return Equals(packet);
            }
            return base.Equals(obj);
        }
        private bool Equals(TestPacket p)
        {
            return VeryData.SequenceEqual(p.VeryData);
        }
    }
}