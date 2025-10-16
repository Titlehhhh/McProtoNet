
namespace McProtoNet.Serialization;

public class BufferOverflowException : Exception
{
    public BufferOverflowException(string message) : base(message)
    {
    }
    public BufferOverflowException()
    {
    }
    public BufferOverflowException(string message, Exception innerException) : base(message, innerException)
    {
    }
}