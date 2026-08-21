# McProtoNet.Transport

Bytes between the socket and the packet. The library frames packets in both directions, compresses them with libdeflate, and encrypts them with AES/CFB8. It knows nothing about protocol phases or protocol versions.

Contents: `Connection/` (MinecraftConnection), `Framing/` (PacketStreamReader, PacketStreamWriter, PacketWriteExtensions), `Compression/`, `Cryptography/`, `Pipelines/`.
