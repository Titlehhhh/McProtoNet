using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static class GenericExtensions
{
    extension(ref MinecraftPrimitiveWriter writer)
    {
        public void WriteArray<T>(ReadOnlySpan<T> arr)
        {
            //TODO
        }

        public void WriteBuffer(ReadOnlySpan<byte> buff, int length)
        {
            
        }

        public void WriteBuffer<TLen>(ReadOnlySpan<byte> buff)
        {
            if (typeof(TLen) == typeof(VarInt))
            {
                writer.WriteVarInt(buff.Length);
                writer.WriteBuffer(buff);
            }
        }
        
        
        public void WriteType<T>(T val)
        {
               
        }
    }
    
    
}