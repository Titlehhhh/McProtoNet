# Version to protocol

Minecraft Java Edition tells versions apart not by name but by a number, the
protocol version. It decides the layout of a packet's fields: the library
keeps several layouts for the same packet, and the number that comes from
the calling code picks the one to use. That number is passed as a parameter
to `SendAsync`, `PacketIo.TryDecode`, `PacketFlow.Dispatch`,
`PacketRegistry.TryResolve`, and in the first handshake packet:

```csharp
await client.SendAsync(
    new HandshakeSb.SetProtocolPacket(Pv, host, port, 2), Pv);
```

## Supported range

The library supports the range from %min_minecraft_version% to
%max_minecraft_version%. In the code, the boundaries are named constants:

```csharp
public const int StartProtocol = V1_16_Protocol;   // 735
public const int LatestProtocol = V26_2_Protocol;  // 776
```

## Mapping table

`MinecraftVersion.FromProtocol` reduces a number to a version. Snapshots and
pre-releases of 1.16.2 collapse to the string 1.16.2, and 1.16.3-rc1
collapses to 1.16.3. The full, constantly updated table of versions and
protocol numbers is on the [Protocol
version](https://minecraft.wiki/w/Protocol_version) page on minecraft.wiki.

| Game version | Protocol |
| --- | --- |
| 1.16 | 735 |
| 1.16.1 | 736 |
| 1.16.2 | 751 |
| 1.16.3 | 753 |
| 1.16.4-1.16.5 | 754 |
| 1.17 | 755 |
| 1.17.1 | 756 |
| 1.18-1.18.1 | 757 |
| 1.18.2 | 758 |
| 1.19 | 759 |
| 1.19.2 | 760 |
| 1.19.3 | 761 |
| 1.19.4 | 762 |
| 1.20-1.20.1 | 763 |
| 1.20.2 | 764 |
| 1.20.3-1.20.4 | 765 |
| 1.20.5-1.20.6 | 766 |
| 1.21-1.21.1 | 767 |
| 1.21.3 | 768 |
| 1.21.4 | 769 |
| 1.21.5 | 770 |
| 1.21.6 | 771 |
| 1.21.7-1.21.8 | 772 |
| 1.21.9-1.21.10 | 773 |
| 1.21.11 | 774 |
| 26.1-26.1.2 | 775 |
| 26.2 | 776 |

## Getting the number from code

`MinecraftVersion` carries named constants, a reverse lookup, and the full
list. There is no need to copy the table into application code:

```csharp
int pv = MinecraftVersion.V1_21_11;      // implicit to int, gives 774
string name = MinecraftVersion.FromProtocol(772).Name;  // "1.21.7–1.21.8"
foreach (var v in MinecraftVersion.AllVersions)
    Console.WriteLine($"{v.Name} -> {v.Protocol}");
```

`FromProtocol` throws `NotSupportedException` on a number outside the
table, including inside the 735-776 range, if it does not match a version.

## A number outside the range

`MinecraftClient` does no check on input. The check comes from the packets
themselves. `SetProtocolPacket`, and any packet with layouts that vary by
version, declares a range:

```csharp
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
```

A number outside the range produces `ProtocolNotSupportException` on the
first attempt to send or parse a packet, before it reaches the network. The
exception carries the type name, the number, and the ranges the type exists
on.

## Next

- [Glossary](02-glossary.md)
- [One build - many versions](../05-packets/05-multiversion.md)
