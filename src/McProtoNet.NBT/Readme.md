# McProtoNet.NBT

NBT for Minecraft Java Edition: big-endian numbers, modified UTF-8 strings, the
file format with a named root and the network format without one. No dependencies.

Read a tag out of a buffer:

```csharp
var reader = new NbtSpanReader(bytes);
var root = reader.ReadAsTag<NbtCompound>(readRootName: true);
int level = root!.Get<NbtInt>("XpLevel")!.Value;
```

Write a tag into any `IBufferWriter<byte>`:

```csharp
var buffer = new ArrayBufferWriter<byte>();
NbtBufferWriter.WriteTag(buffer, root, writeRootName: true);
```

For the network format (nameless root, used since 1.20.2) pass `false` instead.
Since 1.20.3 the root may be any tag, not only a compound. Over a `Stream` use
`NbtReader.ReadTag(stream, readRootName)` and `NbtWriter.WriteTag(stream, tag,
writeRootName)`, or `NbtReader` / `NbtWriter` for pull-style reading and writing.
`ModifiedUtf8` gives the string encoding on its own: span-only, allocation-free,
shaped like `System.Text.Unicode.Utf8`.

Limits: no SNBT and no JSON conversion yet, no gzip or zlib helper yet (wrap the
stream yourself), Java Edition only — there is no little-endian Bedrock mode.
Nesting is capped at 512 levels and strings at 65535 bytes, as in vanilla.

Derived from fNbt (BSD 3-Clause); see THIRD-PARTY-NOTICES.txt in the package.
