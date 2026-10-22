using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace McProtoNet.Benchmark;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct RuntimeZStream
{
    public byte* NextIn;
    public byte* NextOut;
    public nint Msg;
    public nint InternalState;
    public uint AvailIn;
    public uint AvailOut;
}

internal enum RuntimeZlibError
{
    Ok = 0,
    StreamEnd = 1,
    StreamError = -2,
    DataError = -3,
    MemError = -4,
    BufError = -5,
    VersionError = -6
}

internal static unsafe class RuntimeZlibNative
{
    private const string Lib = "System.IO.Compression.Native";

    [DllImport(Lib, EntryPoint = "CompressionNative_InflateInit2_")]
    public static extern RuntimeZlibError InflateInit2_(RuntimeZStream* s, int windowBits);

    [DllImport(Lib, EntryPoint = "CompressionNative_InflateReset2_")]
    public static extern RuntimeZlibError InflateReset2_(RuntimeZStream* s, int windowBits);

    [DllImport(Lib, EntryPoint = "CompressionNative_Inflate")]
    public static extern RuntimeZlibError Inflate(RuntimeZStream* s, int flush);

    [DllImport(Lib, EntryPoint = "CompressionNative_InflateEnd")]
    public static extern RuntimeZlibError InflateEnd(RuntimeZStream* s);

    [DllImport(Lib, EntryPoint = "CompressionNative_DeflateInit2_")]
    public static extern RuntimeZlibError DeflateInit2_(RuntimeZStream* s, int level, int method, int windowBits,
        int memLevel, int strategy);

    [DllImport(Lib, EntryPoint = "CompressionNative_Deflate")]
    public static extern RuntimeZlibError Deflate(RuntimeZStream* s, int flush);

    [DllImport(Lib, EntryPoint = "CompressionNative_DeflateEnd")]
    public static extern RuntimeZlibError DeflateEnd(RuntimeZStream* s);
}

internal sealed unsafe class RuntimeZlibInflater : IDisposable
{
    private const int WindowBits = 15;
    private const int NoFlush = 0;
    private const int Finish = 4;
    private static bool s_hasReset = true;

    private RuntimeZStream* _z;
    private bool _dirty;

    public RuntimeZlibInflater()
    {
        _z = (RuntimeZStream*)NativeMemory.AllocZeroed((nuint)sizeof(RuntimeZStream));
        Check(RuntimeZlibNative.InflateInit2_(_z, WindowBits));
    }

    public int Inflate(in ReadOnlySequence<byte> input, Span<byte> output)
    {
        if (_dirty) Reset();
        _dirty = true;
        fixed (byte* outPtr = output)
        {
            _z->NextOut = outPtr;
            _z->AvailOut = (uint)output.Length;
            bool ended = false;
            foreach (var segment in input)
            {
                if (ended)
                {
                    if (segment.Length > 0) ThrowTrailing();
                    continue;
                }

                fixed (byte* inPtr = segment.Span)
                {
                    _z->NextIn = inPtr;
                    _z->AvailIn = (uint)segment.Length;
                    while (_z->AvailIn > 0)
                    {
                        var r = RuntimeZlibNative.Inflate(_z, NoFlush);
                        if (r == RuntimeZlibError.StreamEnd)
                        {
                            if (_z->AvailIn != 0) ThrowTrailing();
                            ended = true;
                            break;
                        }

                        if (r == RuntimeZlibError.BufError && _z->AvailOut == 0)
                            throw new InvalidOperationException("output too small");
                        Check(r);
                    }

                    _z->NextIn = null;
                }
            }

            if (!ended)
            {
                var fin = RuntimeZlibNative.Inflate(_z, Finish);
                if (fin != RuntimeZlibError.StreamEnd) throw new InvalidOperationException("truncated: " + fin);
            }

            int written = output.Length - (int)_z->AvailOut;
            _z->NextOut = null;
            _z->AvailOut = 0;
            return written;
        }

        static void ThrowTrailing() => throw new InvalidOperationException("trailing bytes after zlib stream");
    }

    private void Reset()
    {
        if (s_hasReset)
        {
            try
            {
                Check(RuntimeZlibNative.InflateReset2_(_z, WindowBits));
                return;
            }
            catch (EntryPointNotFoundException)
            {
                s_hasReset = false;
            }
        }

        RuntimeZlibNative.InflateEnd(_z);
        NativeMemory.Clear(_z, (nuint)sizeof(RuntimeZStream));
        Check(RuntimeZlibNative.InflateInit2_(_z, WindowBits));
    }

    private static void Check(RuntimeZlibError e)
    {
        if (e != RuntimeZlibError.Ok) throw new InvalidOperationException(e.ToString());
    }

    public void Dispose()
    {
        if (_z == null) return;
        RuntimeZlibNative.InflateEnd(_z);
        NativeMemory.Free(_z);
        _z = null;
    }
}

internal sealed unsafe class RuntimeZlibDeflater : IDisposable
{
    private RuntimeZStream* _z;
    private readonly int _level;
    private bool _dirty;

    public RuntimeZlibDeflater(int level)
    {
        _level = level;
        _z = (RuntimeZStream*)NativeMemory.AllocZeroed((nuint)sizeof(RuntimeZStream));
        Init();
    }

    private void Init()
    {
        var r = RuntimeZlibNative.DeflateInit2_(_z, _level, 8, 15, 8, 0);
        if (r != RuntimeZlibError.Ok) throw new InvalidOperationException(r.ToString());
    }

    public int Deflate(ReadOnlySpan<byte> input, Span<byte> output)
    {
        if (_dirty)
        {
            RuntimeZlibNative.DeflateEnd(_z);
            NativeMemory.Clear(_z, (nuint)sizeof(RuntimeZStream));
            Init();
        }

        _dirty = true;
        fixed (byte* i = input)
        fixed (byte* o = output)
        {
            _z->NextIn = i;
            _z->AvailIn = (uint)input.Length;
            _z->NextOut = o;
            _z->AvailOut = (uint)output.Length;
            var r = RuntimeZlibNative.Deflate(_z, 4);
            if (r != RuntimeZlibError.StreamEnd) throw new InvalidOperationException("deflate: " + r);
            return output.Length - (int)_z->AvailOut;
        }
    }

    public void Dispose()
    {
        if (_z == null) return;
        RuntimeZlibNative.DeflateEnd(_z);
        NativeMemory.Free(_z);
        _z = null;
    }
}
