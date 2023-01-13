
using McProtoNet.Core.IO;
using McProtoNet.Core.Packets;
using McProtoNet.Core.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static McProtoNet.Tests.MinecraftProtocolTest;

namespace McProtoNet.Tests
{
    public class TestPacket : MinecraftPacket
    {
        private MemoryStream ms;

        public TestPacket(MemoryStream ms)
        {
            this.ms = ms;
        }
        public TestPacket()
        {

        }

        public byte[] VeryData { get; set; } = new byte[1000];

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            VeryData = stream.ReadByteArray();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            //  Trace.WriteLine("ms.pos: " + ms.Position);
            stream.WriteVarInt(VeryData.Length);
            WriteArr("VarIntLen:", ms.ToArray());
            //  Trace.WriteLine("ms.pos: " + ms.Position);
            stream.Write(VeryData);
            WriteArr("Bytes:", ms.ToArray());
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