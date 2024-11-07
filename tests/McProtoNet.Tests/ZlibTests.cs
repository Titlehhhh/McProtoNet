using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using DotNext.Collections.Generic;
using McProtoNet.Net.Zlib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace McProtoNet.Tests;

[TestClass]
public class ZlibTests
{
    [TestMethod]
    public void MainTest()
    {
        byte[] data = CreateData(500);
        Random r = new Random(27);


        byte[] compressed = Compress(data);
        ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(compressed.AsMemory());
        var libDeflate = sequence.Decompress(500);


        Assert.AreEqual(500, libDeflate.Length);
        CollectionAssert.AreEqual(data, libDeflate.Span.ToArray());
    }

    [TestMethod]
    public void TestAfterErrorLibDeflate()
    {
        Random r = new Random(27);
        byte[] data = CreateData(500);


        byte[] compressed = Compress(data);
        byte[] compressedValid = compressed.AsSpan().ToArray();
        r.Shuffle(compressed);

        ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(compressed.AsMemory());
        
        try
        {
            var libDeflate = sequence.Decompress(500);

            Assert.Fail();
        }
        catch (InvalidOperationException)
        {
        }
        catch (Exception ex)
        {
            Assert.Fail("Bad exception:" + ex);
        }

        sequence = new ReadOnlySequence<byte>(compressedValid);
        var valid = sequence.Decompress(500);

        CollectionAssert.AreEqual(data, valid.Span.ToArray(),
            $"\n\n{string.Join(", ", data)}\n\n{string.Join(", ", valid.Span.ToArray())}");
    }

    [TestMethod]
    public void TestThreadStatic()
    {
        Random r = new Random(27);
        byte[] data = CreateData(500);


        byte[] compressed = Compress(data);

        byte[] outTest = new byte[500];
        var decompressor = LibDeflateCache.RentDecompressor();
        for (int i = 0; i < 500; i++)
        {
            decompressor.Decompress(compressed, outTest, out _);
            
            CollectionAssert.AreEqual(data, outTest);
        }
    }

    private static byte[] Compress(byte[] data)
    {
        MemoryStream ms = new MemoryStream();
        using (ZLibStream zLibStream = new ZLibStream(ms, CompressionLevel.SmallestSize, true))
        {
            zLibStream.Write(data);
        }

        return ms.ToArray();
    }

    private static byte[] CreateData(int size)
    {
        byte[] res = new byte[size];
        for (int i = 0; i < res.Length; i++)
            res[i] = (byte)(i % 8);

        return res;
    }
}