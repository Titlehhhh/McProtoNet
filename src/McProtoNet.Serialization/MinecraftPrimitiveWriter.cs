using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using Cysharp.Text;
using DotNext.Buffers;
using McProtoNet.NBT;

namespace McProtoNet.Serialization;

/// <summary>
/// Represents stack-allocated writer for primitive types of Minecraft
/// </summary>
[StructLayout(LayoutKind.Auto)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref struct MinecraftPrimitiveWriter()
{
    private static readonly MemoryAllocator<byte> s_allocator = ArrayPool<byte>.Shared.ToAllocator();

    private BufferWriterSlim<byte> writerSlim = new(64, s_allocator);

    /// <summary>
    /// Writes a boolean value to the buffer
    /// </summary>
    /// <param name="value">The boolean value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBoolean(bool value)
    {
        CheckDisposed();

        writerSlim.Write(value ? 1 : 0);
    }

    /// <summary>
    /// Writes a signed byte value to the buffer
    /// </summary>
    /// <param name="value">The signed byte value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSignedByte(sbyte value)
    {
        CheckDisposed();
        writerSlim.Write((byte)value);
    }

    /// <summary>
    /// Writes an unsigned byte value to the buffer
    /// </summary>
    /// <param name="value">The unsigned byte value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUnsignedByte(byte value)
    {
        CheckDisposed();
        writerSlim.Write(value);
    }

    /// <summary>
    /// Writes an unsigned short value to the buffer in big-endian format
    /// </summary>
    /// <param name="value">The unsigned short value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUnsignedShort(ushort value)
    {
        CheckDisposed();
        writerSlim.WriteBigEndian(value);
    }

    /// <summary>
    /// Writes a signed short value to the buffer in big-endian format
    /// </summary>
    /// <param name="value">The signed short value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSignedShort(short value)
    {
        CheckDisposed();
        writerSlim.WriteBigEndian(value);
    }

    /// <summary>
    /// Writes a signed integer value to the buffer in big-endian format
    /// </summary>
    /// <param name="value">The signed integer value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSignedInt(int value)
    {
        CheckDisposed();
        writerSlim.WriteBigEndian(value);
    }

    /// <summary>
    /// Writes an unsigned integer value to the buffer in big-endian format
    /// </summary>
    /// <param name="value">The unsigned integer value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUnsignedInt(uint value)
    {
        CheckDisposed();
        writerSlim.WriteBigEndian(value);
    }

    /// <summary>
    /// Writes a signed long value to the buffer in big-endian format
    /// </summary>
    /// <param name="value">The signed long value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSignedLong(long value)
    {
        CheckDisposed();
        writerSlim.WriteBigEndian(value);
    }

    /// <summary>
    /// Writes an unsigned long value to the buffer in big-endian format
    /// </summary>
    /// <param name="value">The unsigned long value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUnsignedLong(ulong value)
    {
        CheckDisposed();
        writerSlim.WriteBigEndian(value);
    }

    /// <summary>
    /// Writes a float value to the buffer in big-endian format
    /// </summary>
    /// <param name="value">The float value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFloat(float value)
    {
        CheckDisposed();
        var val = BitConverter.SingleToInt32Bits(value);
        writerSlim.WriteBigEndian(val);
    }

    /// <summary>
    /// Writes a double value to the buffer in big-endian format
    /// </summary>
    /// <param name="value">The double value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDouble(double value)
    {
        CheckDisposed();
        var val = BitConverter.DoubleToInt64Bits(value);
        writerSlim.WriteBigEndian(val);
    }

    /// <summary>
    /// Writes a UUID (GUID) value to the buffer
    /// </summary>
    /// <param name="value">The UUID value to write</param>
    /// <exception cref="InvalidOperationException">Thrown when the UUID cannot be written to the buffer</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUUID(Guid value)
    {
        CheckDisposed();
        var span = writerSlim.GetSpan(16);

        if (!value.TryWriteBytes(span)) throw new InvalidOperationException("Guid no write");
        writerSlim.Advance(16);
    }

    /// <summary>
    /// Writes a byte buffer to the underlying buffer
    /// </summary>
    /// <param name="value">The byte buffer to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBuffer(ReadOnlySpan<byte> value)
    {
        CheckDisposed();
        writerSlim.Write(value);
    }

    /// <summary>
    /// Writes a nullable VarInt value to the buffer
    /// </summary>
    /// <param name="value">The nullable VarInt value to write</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null</exception>
    public void WriteVarInt(int? value)
    {
        CheckDisposed();
        if (value is null)
            throw new ArgumentNullException("value", "value is null");
        WriteVarInt(value.Value);
    }

    /// <summary>
    /// Writes a VarInt value to the buffer
    /// </summary>
    /// <param name="value">The VarInt value to write</param>
    /// <exception cref="ArithmeticException">Thrown when the VarInt is too big</exception>
#if RELEASE
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void WriteVarInt(int value)
    {
        CheckDisposed();
        Span<byte> data = stackalloc byte[5];

        var unsigned = (uint)value;

        byte len = 0;
        do
        {
            var temp = (byte)(unsigned & 127);
            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            data[len++] = temp;
        } while (unsigned != 0);

        if (len > 5)
            throw new ArithmeticException("Var int is too big");

        writerSlim.Write(data.Slice(0, len));
    }

    /// <summary>
    /// Writes a VarLong value to the buffer
    /// </summary>
    /// <param name="value">The VarLong value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteVarLong(long value)
    {
        CheckDisposed();
        var unsigned = (ulong)value;

        do
        {
            var temp = (byte)(unsigned & 127);

            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;


            writerSlim.Write(temp);
        } while (unsigned != 0);
    }

    private static readonly Encoding encoding = new UTF8Encoding();
    /// <summary>
    /// Writes a string value to the buffer in UTF-8 format
    /// </summary>
    /// <param name="value">The string value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(string value)
    {
        CheckDisposed();
        
        

        int length = encoding.GetByteCount(value);
        WriteVarInt(length);

        Span<byte> span = writerSlim.GetSpan(length);

        if (encoding.TryGetBytes(value, span, out var written))
        {
            writerSlim.Advance(written);
            return;
        }
        
        throw new ArgumentException("Failed to write string to buffer", nameof(value));
        

        
    }

    /// <summary>
    /// Writes an optional NBT tag to the buffer
    /// </summary>
    /// <param name="value">The optional NBT tag to write</param>
    public void WriteOptionalNbt(NbtTag? value)
    {
        CheckDisposed();
        if (value is null)
        {
            WriteBoolean(false);
        }
        else
        {
            WriteBoolean(true);
            WriteNbt(value);
        }
    }

    /// <summary>
    /// Writes an NBT tag to the buffer
    /// </summary>
    /// <param name="value">The NBT tag to write</param>
    public void WriteNbt(NbtTag value)
    {
        CheckDisposed();
        MemoryStream ms = new MemoryStream();
        NbtWriter nbtWriter = new NbtWriter(ms, "");

        nbtWriter.WriteTag(value);

        this.WriteBuffer(ms.ToArray());
    }

    /// <summary>
    /// Checks if the writer has been disposed
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the writer has been disposed</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckDisposed()
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(MinecraftPrimitiveWriter));
    }

    private bool disposed;

    /// <summary>
    /// Gets the written memory buffer and marks the writer as disposed
    /// </summary>
    /// <returns>The written memory buffer</returns>
    /// <exception cref="InvalidOperationException">Thrown when the buffer cannot be detached</exception>
    public MemoryOwner<byte> GetWrittenMemory()
    {
        if (!writerSlim.TryDetachBuffer(out var result))
            throw new InvalidOperationException("Don't detach buffer");

        disposed = true;
        return result;
    }

    /// <summary>
    /// Disposes the writer and releases any resources
    /// </summary>
    public void Dispose()
    {
        if (disposed)
            return;
        disposed = true;

        writerSlim.Dispose();
    }
}