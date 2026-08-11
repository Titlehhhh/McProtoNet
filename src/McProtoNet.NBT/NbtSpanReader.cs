using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;

namespace McProtoNet.NBT;

public ref struct NbtSpanReader
{
    private SpanBinaryReader _reader;
    public int ConsumedCount => _reader.ConsumedCount;

    public NbtSpanReader(ReadOnlySpan<byte> data)
    {
        _reader = new SpanBinaryReader(data);
    }


    public T? ReadAsTag<T>(bool readRootName) where T : NbtTag
    {
        NbtTagType type = ReadTagType();
        if (type == NbtTagType.End)
        {
            return null;
        }

        string? rootName = readRootName ? ReadString() : null;

        if (TypeIsPrimitive(type))
        {
            return (T)ReadPrimitive(type, rootName);
        }

        if (_reader.RemainingCount <= 512) // Recursive
        {
            return ReadRecursive(type, rootName) as T ??
                   throw new InvalidOperationException($"Error cast to {typeof(T)}");
        }

        Stack<NbtTag> stack = new Stack<NbtTag>();
        NbtTag root;

        if (type == NbtTagType.List)
        {
            var listType = ReadTagType();
            var length = _reader.ReadBigEndian32();
            if (length < 0) throw new NbtFormatException($"Negative tag length given: {length}");

            if (TryReadNbtListPrimitive(listType, length, out var resultList))
            {
                resultList.Name = rootName;
                return resultList as T ?? throw new InvalidOperationException($"Error cast to {typeof(T)}");
            }

            var list = new NbtList { Name = rootName };
            stack.Push(list);
            for (int i = 0; i < length; i++)
            {
                var tag = ReadRecursive(listType, null);
                list.Add(tag);
            }

            root = list;
        }
        else // Compound
        {
            var compound = new NbtCompound { Name = rootName };
            stack.Push(compound);

            while (true)
            {
                var nextType = ReadTagType();
                if (nextType == NbtTagType.End) break;

                var name = ReadString();
                if (TypeIsPrimitive(nextType))
                {
                    compound.Add(ReadPrimitive(nextType, name));
                }
                else if (nextType == NbtTagType.List || nextType == NbtTagType.Compound)
                {
                    var tag = ReadRecursive(nextType, name);
                    compound.Add(tag);
                }
            }

            root = compound;
        }

        return (T)root;
    }


    private bool TypeIsPrimitive(NbtTagType type)
    {
        switch (type)
        {
            case NbtTagType.List:
            case NbtTagType.Compound:
                return false;
            default: return true;
        }
    }

    private NbtTag ReadRecursive(NbtTagType type)
    {
        string name = ReadString();
        return ReadRecursive(type, name);
    }

    private NbtTag ReadRecursive(NbtTagType type, string? name)
    {
        if (type == NbtTagType.List)
        {
            NbtTagType listType = ReadTagType();
            int length = _reader.ReadBigEndian32();
            if (length < 0) throw new NbtFormatException($"Negative tag length given: {length}");

            if (TryReadNbtListPrimitive(listType, length, out var resultList))
            {
                resultList.Name = name;
                return resultList;
            }

            NbtList list = new NbtList();
            for (int i = 0; i < length; i++)
            {
                var tag = ReadRecursive(listType, null);
                list.Add(tag);
            }

            list.Name = name;
            return list;
        }

        if (type == NbtTagType.Compound)
        {
            NbtCompound nbtCompound = new NbtCompound();
            nbtCompound.Name = name;
            while (true)
            {
                NbtTagType nextType = ReadTagType();
                if (nextType == NbtTagType.End)
                {
                    return nbtCompound;
                }

                NbtTag tag = ReadRecursive(nextType);
                nbtCompound.Add(tag);
            }
        }

        return ReadPrimitive(type, name);
    }

    private bool TryReadNbtListPrimitive(NbtTagType listType, int length, out NbtList list)
    {
        if (listType == NbtTagType.Byte)
        {
            list = new NbtList();
            ReadOnlySpan<byte> bytes = _reader.Read(length);
            foreach (byte b in bytes)
            {
                list.Add(new NbtByte(b));
            }

            return true;
        }

        if (listType == NbtTagType.Short)
        {
            list = new NbtList();
            ReadOnlySpan<byte> bytes = _reader.Read(length * sizeof(short));
            ReadOnlySpan<short> cast = MemoryMarshal.Cast<byte, short>(bytes);
            if (BitConverter.IsLittleEndian)
            {
                short[] rented = ArrayPool<short>.Shared.Rent(length);
                try
                {
                    BinaryPrimitives.ReverseEndianness(cast, rented.AsSpan(0, length));
                    foreach (var i in rented.AsSpan(0, length))
                        list.Add(new NbtShort(i));
                }
                finally
                {
                    ArrayPool<short>.Shared.Return(rented);
                }
            }
            else
            {
                foreach (var i in cast)
                    list.Add(new NbtShort(i));
            }

            return true;
        }

        if (listType == NbtTagType.Int)
        {
            list = new NbtList();
            ReadOnlySpan<byte> bytes = _reader.Read(length * sizeof(int));
            ReadOnlySpan<int> cast = MemoryMarshal.Cast<byte, int>(bytes);
            if (BitConverter.IsLittleEndian)
            {
                int[] rented = ArrayPool<int>.Shared.Rent(length);
                try
                {
                    BinaryPrimitives.ReverseEndianness(cast, rented.AsSpan(0, length));
                    foreach (var i in rented.AsSpan(0, length))
                        list.Add(new NbtInt(i));
                }
                finally
                {
                    ArrayPool<int>.Shared.Return(rented);
                }
            }
            else
            {
                foreach (var i in cast)
                    list.Add(new NbtInt(i));
            }

            return true;
        }

        if (listType == NbtTagType.Long)
        {
            list = new NbtList();
            ReadOnlySpan<byte> bytes = _reader.Read(length * sizeof(long));
            ReadOnlySpan<long> cast = MemoryMarshal.Cast<byte, long>(bytes);
            if (BitConverter.IsLittleEndian)
            {
                long[] rented = ArrayPool<long>.Shared.Rent(length);
                try
                {
                    BinaryPrimitives.ReverseEndianness(cast, rented.AsSpan(0, length));
                    foreach (var i in rented.AsSpan(0, length))
                        list.Add(new NbtLong(i));
                }
                finally
                {
                    ArrayPool<long>.Shared.Return(rented);
                }
            }
            else
            {
                foreach (var i in cast)
                    list.Add(new NbtLong(i));
            }

            return true;
        }

        if (listType == NbtTagType.Float)
        {
            list = new NbtList();
            ReadOnlySpan<byte> bytes = _reader.Read(length * sizeof(float));

            if (BitConverter.IsLittleEndian)
            {
                ReadOnlySpan<int> cast = MemoryMarshal.Cast<byte, int>(bytes);
                int[] rented = ArrayPool<int>.Shared.Rent(length);
                try
                {
                    BinaryPrimitives.ReverseEndianness(cast, rented.AsSpan(0, length));
                    foreach (var i in MemoryMarshal.Cast<int, float>(rented.AsSpan(0, length)))
                        list.Add(new NbtFloat(i));
                }
                finally
                {
                    ArrayPool<int>.Shared.Return(rented);
                }
            }
            else
            {
                ReadOnlySpan<float> cast = MemoryMarshal.Cast<byte, float>(bytes);
                foreach (var i in cast)
                    list.Add(new NbtFloat(i));
            }

            return true;
        }

        if (listType == NbtTagType.Double)
        {
            list = new NbtList();
            ReadOnlySpan<byte> bytes = _reader.Read(length * sizeof(double));

            if (BitConverter.IsLittleEndian)
            {
                ReadOnlySpan<long> cast = MemoryMarshal.Cast<byte, long>(bytes);
                long[] rented = ArrayPool<long>.Shared.Rent(length);
                try
                {
                    BinaryPrimitives.ReverseEndianness(cast, rented.AsSpan(0, length));
                    foreach (var i in MemoryMarshal.Cast<long, double>(rented.AsSpan(0, length)))
                        list.Add(new NbtDouble(i));
                }
                finally
                {
                    ArrayPool<long>.Shared.Return(rented);
                }
            }
            else
            {
                ReadOnlySpan<double> cast = MemoryMarshal.Cast<byte, double>(bytes);
                foreach (var i in cast)
                    list.Add(new NbtDouble(i));
            }

            return true;
        }

        list = null;
        return false;
    }

    private NbtTag ReadPrimitive(NbtTagType type, string? name)
    {
        if (type == NbtTagType.Byte)
        {
            return new NbtByte(name, _reader.Read());
        }

        if (type == NbtTagType.Short)
        {
            return new NbtShort(name, _reader.ReadBigEndian16());
        }

        if (type == NbtTagType.Int)
        {
            return new NbtInt(name, _reader.ReadBigEndian32());
        }

        if (type == NbtTagType.Long)
        {
            return new NbtLong(name, _reader.ReadBigEndian64());
        }

        if (type == NbtTagType.Float)
        {
            return new NbtFloat(name, ReadFloat());
        }

        if (type == NbtTagType.Double)
        {
            return new NbtDouble(name, ReadDouble());
        }

        if (type == NbtTagType.ByteArray)
        {
            int length = _reader.ReadBigEndian32();
            if (length < 0) throw new NbtFormatException($"Negative array length given: {length}");
            byte[] arr = _reader.Read(length).ToArray();
            return NbtByteArray.CreateFromArray(arr, name);
        }

        if (type == NbtTagType.String)
        {
            return new NbtString(name, ReadString());
        }

        if (type == NbtTagType.IntArray)
        {
            int length = _reader.ReadBigEndian32();
            if (length < 0) throw new NbtFormatException($"Negative array length given: {length}");
            int[] result = new int[length];

            ReadOnlySpan<byte> bytes = _reader.Read(sizeof(int) * length);
            ReadOnlySpan<int> ints = MemoryMarshal.Cast<byte, int>(bytes);

            if (BitConverter.IsLittleEndian)
            {
                BinaryPrimitives.ReverseEndianness(ints, result);
            }
            else
            {
                ints.CopyTo(result);
            }

            return NbtIntArray.CreateFromArray(result, name);
        }

        if (type == NbtTagType.LongArray)
        {
            int length = _reader.ReadBigEndian32();
            if (length < 0) throw new NbtFormatException("Negative array length given: " + length);

            ReadOnlySpan<byte> bytes = _reader.Read(sizeof(long) * length);
            ReadOnlySpan<long> longs = MemoryMarshal.Cast<byte, long>(bytes);
            long[] result = new long[length];
            if (BitConverter.IsLittleEndian)
            {
                BinaryPrimitives.ReverseEndianness(longs, result);
            }
            else
            {
                longs.CopyTo(result);
            }

            return NbtLongArray.CreateFromArray(result, name);
        }

        throw new InvalidOperationException($"Unknown type: {type}({(int)type})");
    }

    private double ReadDouble()
    {
        long l = _reader.ReadBigEndian64();
        return Unsafe.BitCast<long, double>(l);
    }

    private float ReadFloat()
    {
        int l = _reader.ReadBigEndian32();
        return Unsafe.BitCast<int, float>(l);
    }

    private NbtTagType ReadTagType()
    {
        byte type = _reader.Read();
        return type switch
        {
            > (int)NbtTagType.LongArray => throw new NbtFormatException("NBT tag type out of range: " + type),
            _ => (NbtTagType)type
        };
    }

    internal string ReadString()
    {
        // NBT string length is an unsigned big-endian short.
        int len = (ushort)_reader.ReadBigEndian16();
        if (len == 0)
            return "";
        return Encoding.UTF8.GetString(_reader.Read(len));
    }
}
