using System.Runtime.InteropServices;

namespace McProtoNet.Transport.Compression.Native;

/// <summary>Holds the library name and calling convention used by the libdeflate P/Invoke declarations.</summary>
internal static class Constants
{
    public const string DllName = "libdeflate";

    public const CallingConvention CallConv = CallingConvention.Cdecl;
}