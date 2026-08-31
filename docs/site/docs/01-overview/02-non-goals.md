# What the library does not do

McProtoNet covers the protocol: connection, framing, compression, encryption,
and reading and writing packets across all supported versions. Beyond that
starts someone else's territory. Here is what the application must write itself.

## Server login order

The library does not drive handshaking → login → configuration → play on its
own: this state machine lives in the application code. This is deliberate.
Servers with plugins behave differently, and access to every login step is more
useful than a neat `Login()` method that eventually runs into someone else's
configuration.

[Phase and direction](../05-packets/01-phases-and-direction.md) and the
`MinimalBot` example in the repository show how this looks in practice.

## Login to online-mode servers

The library has full encryption support, but requesting a session from Mojang
and verifying the login stay with the application. For now, running a local
server with `online-mode=false` is simpler.

## Game behavior

The library does not make decisions for the bot: where to go, how to avoid
obstacles, when to hit a mob, what to say in chat. It delivers packets in both
directions. The application code gives them meaning.

There are no ready-made scenarios either. Farming, PvP, ore finding, crafting,
trading with villagers - the application writes all of this on top.

## Graphics

There is no rendering of the world, windows, or buttons here. The library works
with bytes and packets. If an interface is needed, it is built separately, on
top.

## Everything around the game

The library does not call third-party services, does not understand mod APIs
like Forge and Fabric, and does not store game data in a database. These things
connect alongside it, through the application's own code.
