namespace McProtoNet.Protocol;

public class CommandNode
{
    //public CommandNodeFlags Flags { get; }

    public int[] Children { get; }

    public int? RedirectNode { get; }

    public string? Name { get; }
}
