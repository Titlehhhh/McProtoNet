
using McProtoNet.Core.IO;
using McProtoNet.Core.Packets;
using McProtoNet.Core.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace McProtoNet.Tests
{
    public class TestPacket : MinecraftPacket
    {
        public int MyProperty1 { get; set; } = 56;
        public int MyProperty2 { get; set; } = 67;
        public int MyProperty3 { get; set; } = 87;
        public byte MyProperty4 { get; set; } = 89;

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            MyProperty1 = stream.ReadInt();
            MyProperty2 = stream.ReadInt();
            MyProperty3 = stream.ReadInt();
            MyProperty4 = stream.ReadUnsignedByte();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteInt(MyProperty1);
            stream.WriteInt(MyProperty2);
            stream.WriteInt(MyProperty3);
            stream.WriteUnsignedByte(MyProperty4);
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
            return this.MyProperty1 == p.MyProperty1
                && this.MyProperty2 == p.MyProperty2
                && this.MyProperty3 == p.MyProperty3
                && this.MyProperty4 == p.MyProperty4;
        }
    }
}