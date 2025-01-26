namespace McProtoNet.FSharp

open System
open QuickProxyNet

module MinecraftClient =

    type MinecraftClientOptions =
        { Username: string
          Version: int
          Host: string
          Port: uint16
          Proxy: Option<IProxyClient> }

    let create (ops: MinecraftClientOptions) =
        new McProtoNet.Client.MinecraftClient(
            Username = ops.Username,
            Host = ops.Host,
            Port = ops.Port,
            Proxy = ops.Proxy.Value
        )
