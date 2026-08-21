using System.Runtime.InteropServices;

namespace McProtoNet.Transport.Compression.Native;

internal static class Constants
{
    public const string DllName = "libdeflate";

    public const CallingConvention CallConv = CallingConvention.Cdecl;
}