# Packet Generation Notes

These notes define the preferred packet format for new multi-version packet generation.

## Preferred packet shape

- Use an abstract packet class with a private nested implementation:
  - `public abstract class XPacket : IClientPacket` (serverbound) or `IServerPacket` (clientbound)
  - `public static XPacket Create() => new Impl();`
  - `internal abstract void ReadPacket(...)` / `WritePacket(...)`
  - `IClientPacket.Serialize` or `IServerPacket.Deserialize` should call the internal method
- Keep payload fields on the abstract class as `get/set` properties.
- Version-specific data should live in small nested structs with **semantic names** (no numeric ranges in names).
- Do not hardcode protocol numbers in the **struct names** (range numbers are allowed in switch cases).
- Read/Write should switch on `protocolVersion` and only serialize fields valid for that range.
- Keep Manual packets untouched.

## Common packets

- `packet_common_*` are payload types reused across stages.
- For now, generate stage-specific packet classes that wrap the common payload.
- IDs are handled later by a marshaller, so do not add ID-specific logic yet.

