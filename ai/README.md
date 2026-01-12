# AI Tooling

This repo provides a single local CLI tool for querying minecraft-data without
loading large JSON files directly.

## mcdata tool

Location:
`tools/agent/mcdata.mjs`

Usage:
```
node tools/agent/mcdata.mjs <command> [options]
```

Output:
- TOON with comma delimiter
- keyFolding safe

### Commands

List:
```
node tools/agent/mcdata.mjs list --roots
node tools/agent/mcdata.mjs list --types
node tools/agent/mcdata.mjs list --packets --state play --direction toClient
```

Schema:
```
node tools/agent/mcdata.mjs schema --type Slot --version 764
node tools/agent/mcdata.mjs schema --packet PacketChat --state play --direction toClient --version 758
```

History:
```
node tools/agent/mcdata.mjs history --type Slot --include-missing
node tools/agent/mcdata.mjs history --packet PacketChat --state play --direction toClient
```

Find:
```
node tools/agent/mcdata.mjs find --field itemId
node tools/agent/mcdata.mjs find --packet Chat
node tools/agent/mcdata.mjs find --type Slot
```

### Notes

- Protocol version uses the numeric protocol value from `info.txt` (e.g. 764).
- `Packet.json` is metadata only; lists come from `Packet*.json`.
- `*_1` aliases are merged automatically (e.g. `Slot_1` + `Slot`).
