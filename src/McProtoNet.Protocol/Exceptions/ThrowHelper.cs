using System.Diagnostics.CodeAnalysis;

namespace McProtoNet.Protocol;

internal static partial class ThrowHelper
{
    public static void ThrowIfProtocolNotSupported<T>(int version)
    {
        if (typeof(T) == typeof(Position))
        {
            if (Position.IsSupportedVersion(version))
            {
                //Throw
            }
        }
        
        // Другие типы должны здесь собираться.
        // Еще генерация исключений, если тип данныз  не поддерживается.
    }
}