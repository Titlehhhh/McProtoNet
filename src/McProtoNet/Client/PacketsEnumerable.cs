using System.Collections.Generic;
using McProtoNet.Abstractions;

namespace McProtoNet.Client
{
    public class TestEnumerable : IAsyncEnumerable<InputPacket>
    {
        public IAsyncEnumerator<InputPacket> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestEnumerator();
        }
    }

    public class TestEnumerator : IAsyncEnumerator<InputPacket>
    {
        public InputPacket Current => throw new NotImplementedException();

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> MoveNextAsync()
        {
            throw new NotImplementedException();
        }
    }
}
