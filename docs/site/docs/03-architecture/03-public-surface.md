# What is visible from outside

There are four layers, but an application usually touches about ten names. Here
they are.

## Client

[`MinecraftClient`](../08-api-reference/McProtoNet/MinecraftClient.md) is
created with
[`MinecraftClientOptions`](../08-api-reference/McProtoNet/MinecraftClientOptions.md)
(host, port, and a proxy if needed), opens a connection through `ConnectAsync`,
and then does a few things:

- `ReadPacketsAsync(token)` - the stream of incoming packets;
- `ReadPacketAsync(token)` - a single packet, when a stream is not needed;
- `SendAsync(packet, protocolVersion)` - send a typed packet;
- `SendRawAsync(id, body)` - send raw bytes;
- `CompressionThreshold` - turn on compression once the server reports it;
- `EnableEncryption(secret)` - turn on the cipher starting from the next frame;
- `DisposeAsync` - close the connection and release the buffers.

## Proxy

An application does not have to open the socket by itself.
`MinecraftClientOptions` takes an `IProxyClient`, with implementations in
[QuickProxyNet](https://github.com/Titlehhhh/QuickProxyNet)
([Joining a server](../04-transport/01-joining-a-server.md)).

## Packets

[`IncomingPacket`](../08-api-reference/McProtoNet/Primitives/IncomingPacket.md)
and
[`OutgoingPacket`](../08-api-reference/McProtoNet/Primitives/OutgoingPacket.md)
are the common currency of every layer. An incoming packet has a number and a
body. The body is a window into a buffer that lives until the next read
([Receive buffer](../04-transport/03-packet-stream.md)).

Each generated packet knows its own identifier and can read and write itself for
a specific protocol version. The
[`PacketRegistry`](../08-api-reference/McProtoNet/Protocol/PacketRegistry.md)
maps a number to a packet descriptor when this lookup is needed by hand.

## Handlers

[`ClientboundHandler`](../08-api-reference/McProtoNet/Protocol/ClientboundHandler.md)
(and
[`ServerboundHandler`](../08-api-reference/McProtoNet/Protocol/ServerboundHandler.md)
for the serverbound direction) is a base class with one method per packet. An
application inherits from it, overrides what it needs, and sets `Phase` itself.
Application code drives the phases
([Phase and direction](../05-packets/01-phases-and-direction.md)). A number the
registry does not know for this version, phase and direction arrives in
`OnUnknown`.

## Bypassing the client

Sometimes the socket is already open, but packets still need decoding.
[`PacketStreamReader`](../08-api-reference/McProtoNet/Transport/Framing/PacketStreamReader.md)
and
[`PacketStreamWriter`](../08-api-reference/McProtoNet/Transport/Framing/PacketStreamWriter.md)
read and write one packet at a time on top of a plain `Stream`, without
[`MinecraftConnection`](../08-api-reference/McProtoNet/Transport/MinecraftConnection.md).

## Around the client

[`SrvResolver`](../08-api-reference/McProtoNet/SrvResolver.md) finds the real
server address from a domain's SRV record.
[`LanServerDetector`](../08-api-reference/McProtoNet/LanServerDetector.md)
listens for broadcast announcements and returns servers open on the local
network.
