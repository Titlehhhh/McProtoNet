using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.IO.Pipelines;
using DotNext;
using DotNext.Buffers;
using DotNext.Collections.Generic;
using DotNext.IO;
using DotNext.IO.Pipelines;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace McProtoNet.Tests;

[TestClass]
public class VarIntTest
{
    [TestMethod]
    public void ReadVarInt()
    {
        Random r = new(23);
        byte[] write = new byte[100];
        r.NextBytes(write);
        
        MemoryStream ms = new MemoryStream();
        using (ZLibStream zlib = new ZLibStream(ms,CompressionLevel.SmallestSize,true))
        {
          
            zlib.Write(write);
        }

        byte[] gg = ms.ToArray();

        Span<byte> one = gg.AsSpan(0, 50);

        Span<byte> two = gg.AsSpan(50);

        Span<byte> output = new byte[100];
        
        ZlibDecompressor decompressor = new ZlibDecompressor();

        var code = decompressor.Decompress(gg, output, out int written);
        
        
        
        var writer = new MinecraftPrimitiveWriter();
       

        int val = 300;

        writer.WriteVarInt(val);
        writer.WriteBuffer(new byte[val]);

        var bytes = writer.GetWrittenMemory().Memory.ToArray();

        SequenceReader reader = IAsyncBinaryReader.Create(bytes);


        var block = reader.ReadBlock(LengthFormat.Compressed, ArrayPool<byte>.Shared.ToAllocator());

        byte[] expected = bytes.AsSpan(val.GetVarIntLength()).ToArray();
        
        byte[] actualToArr = block.Memory.ToArray();
        CollectionAssert.AreEqual(expected, actualToArr,
            $"{string.Join(", ", bytes)}\r\n\r\n{string.Join(", ", block.Memory.ToArray())}");
    }
}

//[TestClass]
//public class ExperimentalTest
//{
//	[TestMethod]
//	public async Task SenderTest()
//	{

//		byte[] for_parsing = new byte[300];

//		int id = Random.Shared.Next(0, 100);

//		Random.Shared.NextBytes(for_parsing);

//		var ms = new MemoryStream();
//		var originalSender = new MinecraftPacketSender();

//		originalSender.SwitchCompression(128);

//		originalSender.BaseStream = ms;

//		var packetStream = new MemoryStream();

//		//await packetStream.WriteVarIntAsync(id);
//		await packetStream.WriteAsync(for_parsing);

//		var buffer = packetStream.ToArray();
//		packetStream.Position = 0;

//		//await originalSender.SendPacketAsync(new (0, buffer.Length, buffer, null));
//		await originalSender.SendPacketAsync(new Packet(id, packetStream));

//		if (ms.Length != 1024)
//		{
//			//Assert.Fail(ms.Length.ToString());
//		}

//		var reader = new MinecraftPacketReaderNew();
//		reader.SwitchCompression(256);
//		reader.BaseStream = ms;
//		ms.Position = 0;
//		var read = await reader.ReadNextPacketAsync();

//		Assert.AreEqual(read.Id, id);

//		//Assert.AreEqual(for_parsing.LongLength, read.Data.Length);

//		CollectionAssert.AreEqual(for_parsing, read.Span.ToArray(), $"{read.Span.ToArray()}");
//	}

//}