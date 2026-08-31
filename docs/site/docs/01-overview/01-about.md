# About the project

McProtoNet is an open-source library, written in C#, for the
[Minecraft Java Edition](https://minecraft.wiki/w/Java_Edition_protocol)
protocol. It handles the connection, parses incoming packets, and sends outgoing
ones - all by itself.

## What people build with it

Bots are the most common case. After that come custom clients and small tools:
query a server, find it by its SRV record, or catch servers on the local
network.

## One build for all versions

The range runs from %min_minecraft_version% to %max_minecraft_version%, and no
separate build is needed for each version. Each packet carries the field layouts
of every protocol version inside itself, and the protocol number given at
connection time picks the right one. Moving to another version changes a number,
not code.

## Speed

Packets are bytes, and the library avoids copying them more than it must.
Receiving avoids copies: a packet arrives as a window into a shared buffer, not
as a fresh array. libdeflate handles compression. The library's own AES, built
on CPU instructions, handles encryption. Primitives are read and written through
`Span<byte>`. The hot path has no reflection, and the assemblies are marked
compatible with NativeAOT.

## What stays outside

Application code builds game behavior on top of the library, not from it. Where
to go, what to attack, what to say in chat - the application code decides these.
The login order also lives in the application code. Details are on the page
[What the library does not do](02-non-goals.md). Read it before planning the
work.

## Next

- [Getting started](../02-getting-started/01-installation.md)
- [First bot](../02-getting-started/02-first-bot.md)
- [Phase and direction](../05-packets/01-phases-and-direction.md)
