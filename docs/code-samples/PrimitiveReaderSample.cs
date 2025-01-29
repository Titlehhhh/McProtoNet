byte[] data = [0x00, 0x15, 0x16, 0x01, 0x54];

MinecraftPrimitiveReader reader = new MinecraftPrimitiveReader(new ReadOnlySpan<byte>(data));
reader.ReadVarInt();
Console.WriteLine($"Remaining: {reader.RemainingCount}");