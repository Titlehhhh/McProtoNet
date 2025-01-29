MinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter();

try
{
    writer.WriteVarInt(111);
    writer.WriteString("Hello");
    var memory = writer.GetWrittenMemory();
    Console.WriteLine(string.Join(", ", memory.Memory));
}
finally
{
    writer.Dispose();
}