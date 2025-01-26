module SimpleBot

open System.Net.Sockets
open McProtoNet.FSharp
open McProtoNet.FSharp.MinecraftClient


let str = new NetworkStream(null,false)

str.AsyncRead
