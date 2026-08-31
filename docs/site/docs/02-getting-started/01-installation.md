# Installation

One package pulls in the whole set:

```
dotnet add package McProtoNet --prerelease
```

The flag is required. All of this documentation describes 2.0, and 2.0 still
ships as prerelease versions. Without `--prerelease`, NuGet installs the latest
stable release from the 1.x branch, which has a different API, and none of the
examples here will build.

.NET 8 or newer is required: the builds target net8.0, net9.0, net10.0, and
net11.0.

## Take only the layer needed

If the whole set is not needed, the packages install separately.

| Package | What it holds |
| --- | --- |
| `McProtoNet` | Glue: connection, SRV records, typed send, LAN server discovery |
| `McProtoNet.Protocol` | Generated packets, types, and base handlers |
| `McProtoNet.Transport` | Framing, compression, encryption |
| `McProtoNet.Primitives` | Primitive reading and writing, packet structures |
| `McProtoNet.NBT` | NBT reading and writing |

The transport pulls in the native package `McProtoNet.Native` 1.0.0, which holds
libdeflate. It does not need to be installed separately.

## Nightly builds

Every push to the `dev` branch ships to the Feedz feed. Versions look like
`2.0.0-preview.4.<number>` and are marked as prerelease, so `--prerelease` is
required. Stable releases ship to nuget.org.

```
dotnet nuget add source https://f.feedz.io/mcprotonet/night/nuget/index.json -n mcprotonet-night
dotnet add package McProtoNet --prerelease
```

The nightly build comes from the latest `dev`: new packages and fixes land there
first, but so do breaks.

## Next

- [First bot](02-first-bot.md)
- [Phase and direction](../05-packets/01-phases-and-direction.md)
