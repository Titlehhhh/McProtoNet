using System.Runtime.CompilerServices;

namespace McProtoNet.Transport.Compression.Native;

/// <summary>Represents the libdeflate options structure passed to the extended allocation entry points.</summary>
internal readonly struct libdeflate_options
{
    private static readonly UIntPtr Size = (nuint)(nint)Unsafe.SizeOf<libdeflate_options>();

    public libdeflate_options(CustomMemoryAllocator.malloc_func malloc, CustomMemoryAllocator.free_func free)
    {
        ArgumentNullException.ThrowIfNull(malloc);
        ArgumentNullException.ThrowIfNull(free);

        this.sizeof_options = Size;
        this.malloc = malloc;
        this.free = free;
    }


    public readonly UIntPtr sizeof_options;


    public readonly CustomMemoryAllocator.malloc_func malloc;


    public readonly CustomMemoryAllocator.free_func free;
}