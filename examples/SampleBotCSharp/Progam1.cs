// using System.Collections.Immutable;
// using System.Net.Sockets;
// using System.Security.AccessControl;
// using System.Text.RegularExpressions;
// using DotNext.Hosting;
// using McProtoNet.Client;
// using McProtoNet.NBT;
// using McProtoNet.Net;
// using McProtoNet.Protocol;
// using McProtoNet.Protocol.Extensions;
// using McProtoNet.Serialization;
//
// namespace SampleBotCSharp;
//
// class Program
// {
//     public static async Task Main(string[] args)
//     {
//         Console.WriteLine("Start");
//         using ConsoleLifetimeTokenSource clts = new();
//
//         var apiKey = Environment.GetEnvironmentVariable("LLM_API_KEY")
//                      ?? throw new InvalidOperationException();
//         OpenRouterService llm = new OpenRouterService(apiKey);
//
//         await llm.LoadAsync("history.json");
//         try
//         {
//             await using var gg = clts.Token.Register(() => { Console.WriteLine("Отмена..."); });
//             var nick = $"AlphaBot";
//
//             try
//             {
//                 await RunBot(llm, nick, clts.Token);
//             }
//             catch (OperationCanceledException) when (clts.IsCancellationRequested)
//             {
//             }
//             catch (Exception exception)
//             {
//                 Console.WriteLine(exception.Message);
//             }
//         }
//         finally
//         {
//             await llm.SaveAsync("history.json");
//         }
//
//         Console.WriteLine("Успешно завершено. Нажмите кнопку для остановки");
//         Console.ReadLine();
//     }
//
//     private static async Task RunBot(
//         OpenRouterService llm,
//         string nick,
//         CancellationToken cancellationToken)
//     {
//         var tcp = new TcpClient();
//
//         await tcp.ConnectAsync("127.0.0.1", 25565, cancellationToken);
//
//         var stream = tcp.GetStream();
//
//         await using var client =
//             PipelinesMinecraftClient.Create(stream, 773);
//
//         var handshake = new HandshakePacket
//         {
//             ProtocolVersion = 773,
//             NextState = 2,
//             ServerHost = "127.0.0.1",
//             ServerPort = 25565
//         };
//         await client.SendPacketAsync(handshake, 0x00, cancellationToken);
//         await client.SendPacketAsync(new LoginStartPacket
//         {
//             Name = nick,
//             UUID = Guid.NewGuid()
//         }, 0x00, cancellationToken);
//
//         await foreach (var p in client.ReadPacketsAsync(cancellationToken))
//         {
//             if (p.Id == 0x03)
//             {
//                 var reader = p.CreateReader();
//                 int threshold = reader.ReadVarInt();
//                 client.CompressionThreshold = threshold;
//             }
//
//             if (p.Id == 0x02)
//             {
//                 await client.SendEmptyPacketAsync(0x03, cancellationToken);
//                 break;
//             }
//         }
//
//
//         await client.SendPacketAsync((ref writer, version) =>
//         {
//             writer.WriteString("ru_RU");
//             writer.WriteSignedByte(16);
//             writer.WriteVarInt(0);
//             writer.WriteBoolean(true);
//             writer.WriteUnsignedByte(127);
//             writer.WriteVarInt(0);
//             writer.WriteBoolean(false);
//             writer.WriteBoolean(false);
//             writer.WriteVarInt(0);
//         }, 0x00, cancellationToken);
//         await foreach (var p in client.ReadPacketsAsync(cancellationToken))
//         {
//             if (p.Id == 0x04) //KeepAlive
//             {
//                 await client.SendPacketAsync((ref writer, _) => { writer.Write(p.Data); }, 0x04, cancellationToken);
//             }
//             else if (p.Id == 0x01) //Payload
//             {
//                 await client.SendPacketAsync((ref writer, _) => { writer.Write(p.Data); }, 0x02, cancellationToken);
//             }
//             else if (p.Id == 0x0E)
//             {
//                 await client.SendPacketAsync((ref writer, _) => { writer.WriteVarInt(0); }, 0x07, cancellationToken);
//             }
//             else if (p.Id == 0x03)
//             {
//                 await client.SendEmptyPacketAsync(0x03, cancellationToken);
//                 break;
//             }
//         }
//
//         SemaphoreSlim sendSem =
//             new SemaphoreSlim(1, 1);
//         await foreach (var p in client.ReadPacketsAsync(cancellationToken))
//         {
//             if (p.Id == 0x2B) //KeepAlive
//             {
//                 await sendSem.WaitAsync(cancellationToken);
//                 try
//                 {
//                     await client.SendPacketAsync(
//                         (ref writer, _) => { writer.Write(p.Data); }, 0x1B, cancellationToken);
//                 }
//                 finally
//                 {
//                     sendSem.Release();
//                 }
//             }
//             else if (p.Id == 0x3F)
//             {
//                 var chat = PlayerChatPacket.Read(p);
//
//                 if (chat.SenderName is NbtString str)
//                 {
//                     if (str.Value.Contains(nick))
//                         continue;
//                 }
//
//                 Task.Run(async () =>
//                 {
//                     try
//                     {
//                         var completed = await llm.SendMessageAsync(chat.Message,
//                             cancellationToken: cancellationToken);
//                         completed = CleanMinecraftMessage(completed);
//                         
//                         Console.WriteLine($"Completed: {completed}");
//                         await sendSem.WaitAsync(cancellationToken);
//                         foreach (var chunk in completed.Chunk(200))
//                         {
//                             var chunkStr = new string(chunk);
//                             
//                             await client.SendChatAsync(chunkStr, cancellationToken);
//                             await Task.Delay(
//                                 Random.Shared.NextTimeSpan(500, 1000), cancellationToken);
//                         }
//                     }
//                     catch (Exception ex)
//                     {
//                         Console.WriteLine(ex);
//                     }
//                     finally
//                     {
//                         sendSem.Release();
//                     }
//                 });
//             }
//         }
//     }
//     
//     public static string CleanMinecraftMessage(string input)
//     {
//         return string.Concat(input
//             .Where(c => !char.IsControl(c) && !char.IsSurrogate(c)));
//     }
//
//
//     
// }
//
//
// public class PlayerChatPacket
// {
//     public class PreviousMessage
//     {
//         public int Id { get; set; }
//         public byte[]? Signature { get; set; }
//
//         public static PreviousMessage[] ReadArr(ref MinecraftPrimitiveReader reader)
//         {
//             int len = reader.ReadVarInt();
//             PreviousMessage[] messages = new PreviousMessage[len];
//             for (int i = 0; i < len; i++)
//             {
//                 var message = new PreviousMessage
//                 {
//                     Id = reader.ReadVarInt(),
//                     Signature = reader.ReadOptional((ref r) => r.ReadBuffer(256))
//                 };
//                 messages[i] = message;
//             }
//
//             return messages;
//         }
//     }
//
//     public int GlobalIndex { get; set; }
//     public Guid Sender { get; set; }
//     public int Index { get; set; }
//
//     public byte[]? Signature { get; set; }
//     public string Message { get; set; }
//
//     public long Timestamp { get; set; }
//     public long Salt { get; set; }
//
//
//     public PreviousMessage[] PreviousMessages { get; set; }
//     public NbtTag? UnsignedContent { get; set; }
//     public int FilterType { get; set; }
//     public long[]? FilterTypeMask { get; set; }
//
//     public NbtTag SenderName { get; set; }
//     public NbtTag? NetworkTargetName { get; set; }
//
//     public static PlayerChatPacket Read(NewInputPacket inPacket)
//     {
//         var reader = inPacket.CreateReader();
//         var packet = new PlayerChatPacket();
//         packet.GlobalIndex = reader.ReadVarInt();
//         packet.Sender = reader.ReadUUID();
//         packet.Index = reader.ReadVarInt();
//         if (reader.ReadBoolean())
//         {
//             packet.Signature = reader.ReadBuffer(256);
//         }
//
//         packet.Message = reader.ReadString();
//         packet.Timestamp = reader.ReadSignedLong();
//         packet.Salt = reader.ReadSignedLong();
//         packet.PreviousMessages = PreviousMessage.ReadArr(ref reader);
//
//         if (reader.ReadBoolean())
//             packet.UnsignedContent = reader.ReadNbtTag(false);
//
//         packet.FilterType = reader.ReadVarInt();
//         if (packet.FilterType == 2)
//             packet.FilterTypeMask = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
//         var id = reader.ReadVarInt();
//         packet.SenderName = reader.ReadNbtTag(false);
//         //packet.NetworkTargetName = reader.ReadOptionalNbtTag(packet.ProtocolVersion);
//         return packet;
//     }
// }
//
