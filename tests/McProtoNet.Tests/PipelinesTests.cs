using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using DotNext;
using DotNext.Buffers;
using McProtoNet.Abstractions;
using McProtoNet.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace McProtoNet.Tests;

[TestClass]
public class PipelinesTests
{
    private static void RandomData(Span<byte> input)
    {
        for (int i = 0; i < input.Length; i++)
            input[i] = (byte)(i % 8);
    }

    [TestMethod]
    public async Task Test()
    {
        Random r = new Random(73);
        var writer = new MinecraftPacketSender();
        List<byte[]> packets = new List<byte[]>();
        writer.SwitchCompression(CompressionThreshold);

        var allocator = ArrayPool<byte>.Shared.ToAllocator();

        MemoryStream ms = new MemoryStream();

        writer.BaseStream = ms;

        for (int i = 0; i < PacketsCount; i++)
        {
            var buffer = allocator.AllocateExactly(r.Next(50, 500));

            //r.NextBytes();
            RandomData(buffer.Span.Slice(5));


            OutputPacket packet = new OutputPacket(buffer);

            packets.Add(packet.Span.Slice(1).ToArray());
            await writer.SendAndDisposeAsync(packet, new CancellationToken());
        }


        ms.Position = 0;

        var reader = new MinecraftPacketPipeReader(PipeReader.Create(ms));
        reader.CompressionThreshold = CompressionThreshold;
        int count = 0;
        await foreach (var packet in reader.ReadPacketsAsync())
        {
            byte[] expected = packets[count];

            byte[] actual = packet.MainData;

            CollectionAssert.AreEqual(expected, actual, $"Packet #{count}");

            packet.Dispose();
            count++;
            if (count >= PacketsCount)
            {
                break;
            }
        }
    }


    public int CompressionThreshold { get; set; } = 128;

    public int PacketsCount { get; set; } = 1000;
}