# McProtoNet

The glue package: what a bot needs and what neither the transport nor the packet layer gives alone. Connects to a server (SRV lookup, TCP), sends typed packets with a protocol version, and finds LAN servers.

Builds on **McProtoNet.Transport** (framing, compression, encryption) and **McProtoNet.Protocol** (packets and types).

## Through a proxy

Set `MinecraftClientOptions.Proxy` and the client connects through the proxy instead of opening its own socket. Any `IProxyClient` from **QuickProxyNet** works — HTTP, SOCKS4/4a/SOCKS5, and the share-link schemes.

```csharp
var proxy = ProxyClientFactory.Instance.Create("socks5://user:pass@127.0.0.1:1080");
await using var client = new MinecraftClient(new MinecraftClientOptions { Host = "mc.example.com", Proxy = proxy });
await client.ConnectAsync();
```
