using System.Net.Sockets;
using System.Threading.Channels;
using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet.Client.New;
using McProtoNet.Net;
using MinecraftClient = McProtoNet.Client.New.MinecraftClient;

MinecraftClientBuilder builder = new();

builder
    .WithHost("title")
    .WithPort(25565)
    .WithVersion(MinecraftVersion.Latest);

MinecraftClient client = builder.Build();

await client.ConnectAsync();



