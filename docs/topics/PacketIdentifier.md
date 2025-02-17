# PacketIdentifier

`PacketIdentifier` представляет собой мета-информацию о пакете. 
Это класс, и может возникнуть впечатление, что он каждый раз создается заново при вызове метода 
`GetPacketId`. Однако это не так.

Каждый класс, описывающий пакет, просто возвращает существующий статический экземпляр из классов,
таких как `ClientConfigurationPacket`, `ClientLoginPacket`, `ServerPlayPacket` и другие. 
Эти классы напоминают перечисления, где каждый вариант — это статическое поле `readonly`.
Чтобы было понятнее, вот пример того, как это реализуется:

```C#
public static class ServerPlayPacket
{
    public static readonly PacketIdentifier Abilities = new(0, nameof(Abilities), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier AcknowledgePlayerDigging =
        new(1, nameof(AcknowledgePlayerDigging), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ActionBar = new(2, nameof(ActionBar), PacketState.Play,
        PacketDirection.Clientbound);
    // ...
}
```

```C#
public class AbilitiesPacket : IServerPacket
{
    public PacketIdentifier GetPacketId()
    {
        return ServerPlayPacket.Abilities;
    }
    // Deserialize...
}
```