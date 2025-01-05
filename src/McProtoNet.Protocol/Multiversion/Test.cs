namespace McProtoNet.Protocol.Multiversion;

public class TestProtocol
{
    public static async Task ClientCode()
    {
        TestProtocol testProtocol = new TestProtocol();

        await testProtocol.SendChatPacket.Multi("Hello");
    }

    private readonly ChatPacketSender _chatPacketSender;

    public ChatPacketSender SendChatPacket => _chatPacketSender;
}

public struct ChatPacketSender
{
    public ValueTask Multi(string message)
    {
        return ValueTask.CompletedTask;
    }
}