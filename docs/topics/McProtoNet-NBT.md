# McProtoNet.NBT

Здесь собраны инструменты для работы с NBT.

## Создание NBT
```C#
var tag = new NbtCompound("root")
{
    new NbtShort("myShort", 1234),
    new NbtInt("myInt",1),
    new NbtLong("myLong",1233L),
    new NbtFloat("myFloat",0.4F),
    new NbtDouble("myDouble",234234.123123D),
    new NbtByteArray("myByteArray", [3,43,76]),
    new NbtString("myString","Hello World!"),
    new NbtIntArray("myIntArray",[34,25,56456456,43]),
    new NbtLongArray("myLongArray",[2342342,234234234,23423423423424323L])
};
```

## Сериализация

```C#
MemoryStream ms = new MemoryStream();
NbtWriter writer = new NbtWriter(ms, rootTagName:"root", bigEndian: true);
writer.WriteTag(tag);
```

