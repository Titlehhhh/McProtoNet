# McProtoNet

The glue package: what a bot needs and what neither the transport nor the packet layer gives alone. Connects to a server (SRV lookup, TCP), sends typed packets with a protocol version, and finds LAN servers.

Builds on **McProtoNet.Transport** (framing, compression, encryption) and **McProtoNet.Protocol** (packets and types).
