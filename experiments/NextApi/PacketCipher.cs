// EXPERIMENT (McProtoNet.Next) — the cipher abstraction the client owns itself.
// Review follow-up: the expert EnableEncryption overload used to take BouncyCastle's
// IBufferedCipher, welding the new public surface to a library the hardening plan
// (question Д5) is considering replacing. This class is the library-owned seam:
// the built-in AES/CFB8 path implements it internally, and an adapter for any
// external crypto library is a few lines of consumer code, not a surface type.

#nullable enable

using System;

namespace McProtoNet.Next;

/// <summary>
/// A byte-stream cipher the transport applies in place at its pump boundary. Implement it to
/// supply custom encryption through
/// <see cref="MinecraftClient.EnableEncryption(PacketCipher, PacketCipher)"/>; the built-in
/// AES-128/CFB8 pair used by <see cref="MinecraftClient.EnableEncryption(ReadOnlySpan{byte})"/>
/// is an internal implementation of this class.
/// </summary>
/// <remarks>
/// <para>
/// The contract is that of a stream cipher with byte granularity, like CFB8: the
/// transformation maps input length 1:1 to output length and works in place on the supplied
/// buffer. Block ciphers that require padding or whole-block input do not fit.
/// </para>
/// <para>
/// Threading: the transport calls <see cref="Transform"/> from a single pump per direction, so
/// one instance is only ever used from one task at a time — but the inbound and outbound
/// instances run concurrently with each other, which is why encryption always takes a pair.
/// </para>
/// <para>
/// Ownership: once handed to the client, the instances belong to it — the client disposes
/// them when it is disposed.
/// </para>
/// </remarks>
public abstract class PacketCipher : IDisposable
{
    /// <summary>
    /// Transforms <paramref name="buffer"/> in place: ciphertext in, plaintext out for the
    /// inbound instance; plaintext in, ciphertext out for the outbound one.
    /// </summary>
    /// <param name="buffer">The bytes to transform. May be empty; may be any length.</param>
    public abstract void Transform(Span<byte> buffer);

    /// <summary>Releases the resources of the cipher.</summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases the resources of the cipher. The base implementation does nothing.</summary>
    /// <param name="disposing"><see langword="true"/> when called from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
    }
}
