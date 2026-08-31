# One build - many versions

The "About the project" page states this in two paragraphs: one range of
protocols, one build instead of a rebuild for each game version. This page
covers how it works in the packet layer: which attributes appear on a
generated packet, where the field layout comes from, and what happens when
code asks for a version the build does not have.

## Attributes that build a packet

Every packet carries three attributes: `ProtocolSupport` - the range of
versions where the packet exists; `Packet` - the manifest key, phase, and
direction; `PacketField` - a field, and if it does not span the whole
range, also `Group` and the `From`/`To` bounds.

```csharp
[ProtocolSupport(
    MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.open_window",
    PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("WindowId", "int")]
[PacketField("InventoryType", "int")]
[PacketField("WindowTitleJson", "string", Group = "VUntil764", To = 764)]
[PacketField("WindowTitle", "NbtTag", Group = "V765_Last", From = 765)]
```

`WindowId` and `InventoryType` carry no `Group` - they are shared fields,
present in every version of the range. `WindowTitleJson` and `WindowTitle`
are the same field in meaning, the title of the window being opened, but on
older versions it is a JSON string, and on newer versions it is an NBT tag:
different types, and neither belongs in the shared part.

`VersionRangeGenerator`, the only Roslyn generator in the project, reads
these attributes: from `ProtocolSupport` it builds
`IsSupportedVersion(int)` and a helper that throws. It does not build the
field layout itself - that is already assembled in the packet code.

## A layer instead of a separate packet

The library does not create one class per version. A packet has one class
for the whole range, and versions that diverge in their fields settle into
nested structures - layers:

```csharp
public sealed partial record OpenWindowPacket(
    int WindowId, int InventoryType,
    OpenWindowPacket.VUntil764Layer? VUntil764 = null,
    OpenWindowPacket.V765_LastLayer? V765_Last = null)
    : IPacket<OpenWindowPacket>, IPacket
{
    public readonly record struct VUntil764Layer(string WindowTitleJson);
    public readonly record struct V765_LastLayer(NbtTag WindowTitle);
```

The layer name follows the range: `VUntil764` runs from the start up to and
including that version, `V765_Last` runs from it to the end, and
`V759_765` (in other packets) is a segment between two versions. Shared
fields live in the packet record, version-specific fields live in their own
layer, and exactly one layer is ever filled.

## Parsing follows the version number, not the class

`Read` and `Write` branch on `protocolVersion` and build or read exactly
the layer that fits. Here is the branch for versions where the
`WindowTitleJson` field is gone and the title arrives as an NBT tag:

```csharp
if (protocolVersion >= 765)
{
    var windowId = reader.ReadVarInt();
    var inventoryType = reader.ReadVarInt();
    var windowTitle = reader.ReadNbtTag(false)!;
    return new OpenWindowPacket(windowId, inventoryType,
        V765_Last: new V765_LastLayer(windowTitle));
}
```

The branch for older versions is a mirror: the same `WindowId` and
`InventoryType`, but `ReadString` instead of `ReadNbtTag`, and the layer is
built as `VUntil764`. Application code never sees this branch: it gets a
ready `OpenWindowPacket` and checks which layer is filled. Writing the
wrong layer for the wrong version is blocked by `Write` with a
`WrongLayerException` - a guard against a manual mistake, not the normal
path.

## The packet number changes more often than the packet itself

The window title changed once across the whole range, while the number of
the `open_window` packet jumps on almost every version - different things.
The number is not known by the library as a whole, but by the specific
type, through its own `TryGetPacketId`:

```csharp
if (protocolVersion >= 762 && protocolVersion <= 763)
{
    id = 0x30;
    return true;
}

if (protocolVersion >= 764 && protocolVersion <= 765)
{
    id = 0x31;
    return true;
}
```

A single `OpenWindowPacket` holds more than a dozen ranges like this.
`PacketRegistry.g.cs` keeps the summary for every packet (`IdRange` for a
range, `PacketDescriptor` for a packet), and on top of it sit flat
number-to-ordinal tables by phase, direction, and version. Code uses them
to find the packet type from a foreign number - the layout of the address
is on the "Phase and direction" page.

## When a packet has no such version

There are two kinds of rejection. A version outside the whole
`ProtocolSupport` range: `Read`, `Write`, and typed sending throw
`ProtocolNotSupportException` with the version and the list of supported
ranges. A version inside the range but with no matching branch in
`Read`/`Write` is a gap in the field layout, not in version support, and
both methods throw `NotSupportedException` with the text "has no wire
layout for protocol version". When the packet number itself is not found
for a foreign version, it never reaches these exceptions:
`PacketRegistry.TryResolve` returns a failure, and the packet goes to
`Unknown` - see "Phase and direction".

## What to change in application code on a version move

Usually only a number: the protocol version that the code passes on
connect. Dispatch, packet numbers, and layer selection recompute themselves
from the new number, with no changes anywhere else in the code.

Changes are needed where application code touches a version layer
directly - reads `packet.VUntil764` instead of the shared fields. On a
newer version, that property becomes `null`, and the needed field moves to
`V765_Last`, not always of the same type: a JSON string and an NBT tag do
not convert into each other. Code that reads only the shared fields of a
packet never sees the version layers at all.

## What multi-version support does not cover

The range is `%min_minecraft_version%` - `%max_minecraft_version%`, and it
is the range of a specific build, not the whole history of the protocol: a
version outside its bounds gets `ProtocolNotSupportException`, not an
approximate parse. Multi-version support here is about the shape of the
packet on the wire, not about game data: the library does not align
numeric block and item ids between versions. And the layer layout does not
hide the version either - code that needs version-specific fields must
know which layer is filled.

## Next

- [From a raw packet](03-from-raw-packet.md)
- [Phase and direction](01-phases-and-direction.md)
- [Version to protocol](../07-reference/01-version-to-protocol.md)
