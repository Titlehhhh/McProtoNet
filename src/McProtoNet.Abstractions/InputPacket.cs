using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DotNext.Buffers;
using McProtoNet.Serialization;

namespace McProtoNet.Abstractions
{
    /// <summary>
    /// Represents an input packet received from the Minecraft protocol.
    /// This structure holds the packet ID and its associated data.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct InputPacket : IDisposable
    {
        /// <summary>
        /// Gets the unique identifier of the packet.
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// Gets the read-only memory containing the packet data.
        /// </summary>
        public readonly ReadOnlyMemory<byte> Data;

        private MemoryOwner<byte> owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputPacket"/> struct with a specified packet ID and memory owner.
        /// </summary>
        /// <param name="id">The unique identifier of the packet.</param>
        /// <param name="owner">The memory owner containing the packet data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InputPacket(int id, MemoryOwner<byte> owner)
        {
            this.owner = owner;
            Id = id;
            Data = this.owner.Memory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputPacket"/> struct by extracting the packet ID from the provided memory.
        /// </summary>
        /// <param name="owner">The memory owner containing the packet data.</param>
        /// <param name="offset">The offset in memory from where the packet data begins. Default is 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InputPacket(MemoryOwner<byte> owner, int offset = 0)
        {
            this.owner = owner;
            Memory<byte> mainData = this.owner.Memory.Slice(offset);
            Id = Extensions.ReadVarInt(mainData.Span, out var offsetId);
            Data = mainData.Slice(offsetId);
        }

        /// <summary>
        /// Releases the resources used by this packet.
        /// </summary>
        public void Dispose()
        {
            owner.Dispose();
        }
    }
}
