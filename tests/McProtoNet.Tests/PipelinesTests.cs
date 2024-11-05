using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using DotNext.Buffers;
using McProtoNet.Abstractions;
using McProtoNet.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace McProtoNet.Tests;

[TestClass]
public class PipelinesTests
{
    [TestMethod]
    public async Task Test()
    {
        Random r = new Random(73);
        var writer = new MinecraftPacketSender();
        List<byte[]> packets = new List<byte[]>();
        writer.SwitchCompression(CompressionThreshold);

        var allocator = ArrayPool<byte>.Shared.ToAllocator();
        using (var fs = File.OpenWrite("data.bin"))
        {
            writer.BaseStream = fs;

            for (int i = 0; i < PacketsCount; i++)
            {
                var buffer = allocator.AllocateExactly(PacketLenght);
                r.NextBytes(buffer.Span.Slice(5));

                OutputPacket packet = new OutputPacket(buffer);

                packets.Add(packet.Span.Slice(1).ToArray());
                await writer.SendAndDisposeAsync(packet, new CancellationToken());
            }
        }

        await using var fs1 = File.OpenRead("data.bin");

        var reader = new MinecraftPacketPipeReader(PipeReader.Create(fs1));
        reader.CompressionThreshold = CompressionThreshold;

        for (int i = 0; i < PacketsCount; i++)
        {
            var inputPacket = await reader.ReadPacketAsync();

            byte[] expected = packets[i];

            byte[] actual = inputPacket.Data.ToArray();

            CollectionAssert.AreEqual(expected, actual, $"Packet #{i}");

            inputPacket.Dispose();
        }
    }

    public int PacketLenght { get; set; } = 165;
    public int CompressionThreshold { get; set; } = 128;

    public int PacketsCount { get; set; } = 10;
}