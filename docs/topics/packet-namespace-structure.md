# Структура пространств имён пакетов

Все классы, отвечающие за пакеты, хранятся в пространстве имён `McProtoNet.Protocol.Packets.*`.

Серверные пакеты режима конфигурации хранятся в `McProtoNet.Protocol.Packets.Configuration.Clientbound`,
клиентские пакеты режима **Play** в `McProtoNet.Protocol.Packets.Play.Serverbound` и так далее.

**Полная структура**
```txt
McProtoNet
│
└───Protocol
    │
    └───Packets
        │
        ├───Handshaking
        │   └───Serverbound
        │
        ├───Login
        │   ├───Clientbound
        │   └───Serverbound
        │
        ├───Configuration
        │   ├───Clientbound
        │   └───Serverbound
        │
        └───Play
            ├───Clientbound
            └───Serverbound
```