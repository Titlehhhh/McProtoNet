# Encryption and compression

A connection opens raw: without compression and without encryption. Both
layers turn on mid-session, each through its own action in the
application code, and both apply to the transport as a whole - to both
directions at once.

## Compression

The server reports the compression threshold in a `LoginCompressPacket`.
The application code copies the number into `CompressionThreshold` on
`MinecraftClient` (`MinecraftConnection` has the same property;
`MinecraftClient` only forwards it). A negative value means compression
is off, and that is also the initial state of a connection. A new value
takes effect from the next frame, not the current one.

The threshold decides the fate of each outgoing packet on its own. A
packet shorter than the threshold goes out as is, but the frame gains an
extra VarInt before the body - 0, meaning "not compressed". A packet not
shorter than the threshold gets compressed, and the same spot in the
frame holds a VarInt with the original (uncompressed) size. On the
incoming side, zero in this field signals to unpack the packet as is,
and a value other than zero says how many bytes to decompress. The
protocol describes this format on the
[With compression](https://minecraft.wiki/w/Java_Edition_protocol/Packets#With_compression)
page.

The compression engine is libdeflate, from the `McProtoNet.Native`
package. Managed code calls it through `DllImport`:
`libdeflate_zlib_compress`, `libdeflate_zlib_decompress`, and the paired
`alloc`/`free` functions for the compressor and decompressor handles.
The native call replaces the built-in `ZLibStream` for speed, and the
format does not change because of it: the wire still carries the same
zlib stream.

The compressor and decompressor handles are not created per packet:
`LibDeflateCache` keeps one of each per thread (`[ThreadStatic]`) and
reuses them for every frame on that thread. The compressor's compression
level is fixed in code - 4 out of the libdeflate range 0-12.

## Encryption

The cipher is AES/CFB8: the same mode the protocol describes on the
[Encryption](https://minecraft.wiki/w/Java_Edition_protocol/Encryption)
page. The shared secret that both sides exchange through
`EncryptionRequestPacket` and `EncryptionResponsePacket` serves as both
the key and the initialization vector - `PacketCipher.SharedSecretLength`
requires exactly 16 bytes, an AES-128 key.

`PacketCipher.CreateEncryptor` and `CreateDecryptor` build one cipher
per direction - `MinecraftConnection.EnableEncryption` creates both at
once and hands them to the frame reader and writer. Encryption turns on
from the next frame in both directions and does not turn off again:
calling `EnableEncryption` a second time on the same connection throws
an exception.

## Hardware paths

A static factory picks the concrete `PacketCipher` implementation based
on processor support, in this check order:

```csharp
if (AesCfb8HardwareCipher.IsSupported)
    return new AesCfb8HardwareCipher(sharedSecret, sharedSecret, encrypting);

if (AesCfb8ArmCipher.IsSupported)
    return new AesCfb8ArmCipher(sharedSecret, sharedSecret, encrypting);

return new AesCfb8Cipher(sharedSecret, sharedSecret, encrypting);
```

`AesCfb8HardwareCipher` uses x86 AES-NI intrinsics on top of
SSE2/SSSE3/SSE4.1. `AesCfb8ArmCipher` uses AES and AdvSimd (NEON) on
ARM64, with its own AES-128 key expansion (`AesKeySchedule`), because the
platform `Aes.Create()` does not cover this path. When neither is
supported, `AesCfb8Cipher` takes over - a wrapper over the platform
`Aes` with `EncryptCfb`/`DecryptCfb` and `feedbackSizeInBits: 8`. Both
intrinsic implementations also parallelize decryption in 16-byte blocks
per pass, instead of going byte by byte the way CFB8 itself requires.

## Order on the stream

On the outside, closer to the socket, sits the cipher. On the inside
sits the whole frame, including the length varint and, if present, the
compression envelope. When writing, `BufferedPacketWriter` first
completes the frame (length, and compression if needed), and only then
encrypts the resulting bytes in place. When reading, it works the other
way: raw bytes from the socket first pass through the cipher, and only
then does the code parse the frame length in them and, if needed,
decompress the body.

## What the application does

Compression and encryption do not switch on by themselves - the
application code turns on both, one line for each:

```csharp
client.CompressionThreshold = packet.Threshold;
client.EnableEncryption(secret);
```

Everything else - the choice of engine, the hardware path, the frame
format, and the order of application on the stream - stays inside the
transport.

## Next

- [Frames](02-framing.md)
- [Joining a server](01-joining-a-server.md)
- [First bot](../02-getting-started/02-first-bot.md)
