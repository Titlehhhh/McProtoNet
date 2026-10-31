using System.Runtime.InteropServices;

namespace McProtoNet.Transport.Compression.Native;

/// <summary>Declares the libdeflate compression entry points.</summary>
internal static class Compression
{
    // Allocation returns a raw pointer on purpose: the factory that turns it into a SafeHandle lives
    // on LibDeflateCompressorHandle, so a failed allocation never reaches a handle instance.
    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv, ExactSpelling = true)]
    public static extern IntPtr libdeflate_alloc_compressor(int compression_level);

    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv, ExactSpelling = true)]
    public static extern IntPtr libdeflate_alloc_compressor_ex(int compression_level,
        in libdeflate_options options);


    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv, ExactSpelling = true)]
    public static extern UIntPtr libdeflate_deflate_compress(LibDeflateCompressorHandle compressor, in byte @in,
        UIntPtr in_nbytes, ref byte @out, UIntPtr out_nbytes_avail);


    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv, ExactSpelling = true)]
    public static extern UIntPtr libdeflate_deflate_compress_bound(LibDeflateCompressorHandle compressor,
        UIntPtr in_nbytes);

    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv, ExactSpelling = true)]
    public static extern UIntPtr libdeflate_zlib_compress(LibDeflateCompressorHandle compressor, in byte @in,
        UIntPtr in_nbytes, ref byte @out, UIntPtr out_nbytes_avail);


    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv, ExactSpelling = true)]
    public static extern UIntPtr libdeflate_zlib_compress_bound(LibDeflateCompressorHandle compressor,
        UIntPtr in_nbytes);


    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv, ExactSpelling = true)]
    public static extern UIntPtr libdeflate_gzip_compress(LibDeflateCompressorHandle compressor, in byte @in,
        UIntPtr in_nbytes, ref byte @out, UIntPtr out_nbytes_avail);


    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv, ExactSpelling = true)]
    public static extern UIntPtr libdeflate_gzip_compress_bound(LibDeflateCompressorHandle compressor,
        UIntPtr in_nbytes);


    // The free entry point takes the raw pointer: ReleaseHandle must not depend on the SafeHandle it
    // is closing.
    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv, ExactSpelling = true)]
    public static extern void libdeflate_free_compressor(IntPtr compressor);
}
