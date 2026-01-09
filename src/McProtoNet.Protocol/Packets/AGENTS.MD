# Packet Generation Notes

These notes define the preferred packet format for new multi-version packet generation.

## Preferred packet shape

- Use a single sealed packet class (no nested impl, no Create method, no base class).
- All packet classes MUST be `partial` to allow attributes/source-gen additions.
- Keep version-specific fields inside `Fields` structs when required.
- Do not generate empty Fields structs.
- If a packet supports a single continuous range, do NOT add a `Vxxx_yyyFields` struct.
- `Serialize` / `Deserialize` methods must be `internal` on the packet.
- Avoid extra helper Read/Write methods unless they are required to keep complex
  nested structures readable; any helpers must be `private` and scoped to the packet.
- Implement `IPacket.Serialize` / `IPacket.Deserialize` explicitly and route to the internal methods.
- Use protocol-version switches inside `Serialize` / `Deserialize`.
- When a range boundary equals the global start or latest version, use
  `MinecraftVersion.StartProtocol` / `MinecraftVersion.LatestProtocol` in the switch.
- Use `ThrowHelper` for protocol support failures; do not instantiate exceptions directly.
- Prefer generic `ReadArray<T>` / `WriteArray<T>` helpers when emitting array fields.
- Packets MUST be generated directly from `minecraft-data` (no inference).
- The authoritative packet list comes from `minecraft-data/<state>/<direction>/Packet*.json`;
  do NOT use `Packet.json` for packet enumeration (it is metadata only).
- `Manual` packet folders are legacy; regenerate and relocate those packets into the normal folder structure.
- New packet files must be saved as UTF-8 with BOM to match project encoding expectations.

## Fields naming

- Use `VFirst_###Fields` instead of hardcoding the first protocol number.
- Use `V###_LastFields` instead of hardcoding the last protocol number.

## Common packets

- `packet_common_*` are payload types reused across stages.
- Packets that wrap common payloads should store the payload as a normal property when the
  packet has a single supported range.
- IDs are handled later by a marshaller, so do not add ID-specific logic yet.
- For `bitfield` layouts, treat the last listed field as the least-significant bits.
- `entityMetadataLoop` uses `0xFF` as the terminator key and does not include payload for it.
