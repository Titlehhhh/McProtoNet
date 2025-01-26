using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DotNext.Buffers;

namespace McProtoNet.Abstractions;

[StructLayout(LayoutKind.Auto)]
public readonly struct InputPacket : IDisposable
{
    public readonly int Id;
    public readonly Memory<byte> Data;

    internal readonly byte[] MainData;

    private readonly MemoryOwner<byte> owner;
    private readonly bool isMemoryPool;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InputPacket(int id, MemoryOwner<byte> owner)
    {
        this.owner = owner;
        Id = id;
        Data = this.owner.Memory;
        isMemoryPool = true;
        // MainData = Data.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InputPacket(MemoryOwner<byte> owner, int offset = 0)
    {
        this.owner = owner;
        Memory<byte> mainData = this.owner.Memory.Slice(offset);

        Id = ReadVarInt(mainData.Span, out int offsetId);
        Data = mainData.Slice(offsetId);
        isMemoryPool = true;
        //MainData = Data.ToArray();
    }

    public InputPacket(ReadOnlySequence<byte> data)
    {
        scoped var r = new SequenceReader<byte>(data);

        TryReadVarInt(ref r, out int id, out int offset);
        this.Id = id;
        //MainData = data.Slice(offset).ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ReadVarInt(Span<byte> data, out int len)
    {
        var numRead = 0;
        var result = 0;
        byte read;
        do
        {
            read = data[numRead];

            var value = read & 0b01111111;
            result |= value << (7 * numRead);

            numRead++;
            if (numRead > 5) throw new ArithmeticException("VarInt too long");
        } while ((read & 0b10000000) != 0);

        //data = data.Slice(numRead);


        len = numRead;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadVarInt(ref SequenceReader<byte> reader, out int res, out int length)
    {
        var numRead = 0;
        var result = 0;
        byte read;
        do
        {
            if (reader.TryRead(out read))
            {
                var value = read & 127;
                result |= value << (7 * numRead);

                numRead++;
                if (numRead > 5) throw new ArithmeticException("VarInt too long");
            }
            else
            {
                res = 0;
                length = -1;
                return false;
            }
        } while ((read & 0b10000000) != 0);


        res = result;
        length = numRead;
        return true;
    }

    public void Dispose()
    {
        if (isMemoryPool)
            owner.Dispose();
    }
}