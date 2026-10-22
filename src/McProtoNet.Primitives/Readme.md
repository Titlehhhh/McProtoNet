# McProtoNet.Primitives

**McProtoNet.Primitives** encodes and decodes the primitives of the Minecraft protocol. It gives a fast and flexible API over the binary wire format:

- Numbers in BigEndian order, as the protocol specifies, with no manual byte-order handling.

- VarInt, VarLong, UUID, strings and NBT (Named Binary Tag).

- `IncomingPacket` and `OutgoingPacket` — the id plus body pair that every floor above uses.
