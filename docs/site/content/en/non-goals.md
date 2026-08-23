# What the library does not do

McProtoNet takes the protocol off your hands: framing, compression, encryption, serialization of every packet, multi-version support, and low-level reading and writing. Some things stay with you, by design, and will not move into the library soon.

## 1. Game logic and AI

The library makes no decisions for you:

- how to move through the world (pathfinding, obstacle avoidance);
- when to attack a mob or interact with a block;
- how to react to events (chat, game mechanics).

The behavior of your bot or client is yours to write.

## 2. User interface

There is no windowing, no buttons, no 3D rendering. If you want a GUI around your bot, bring your own — the library works under the hood.

## 3. Integrations with other APIs

Not included:

- parsing data from third-party services (maps, plugins);
- mod APIs (Forge, Fabric);
- databases for game data.

## 4. Ready-made solutions for popular tasks

You will not find here:

- templates for an auto-farm or a PvP bot;
- algorithms for ore search or building;
- out-of-the-box emulation of game actions (crafting, villager trading).
