using System.IO.Pipelines;
using McProtoNet.Transport.Pipelines;
using McProtoNet.Tests.Infrastructure;

namespace McProtoNet.Tests.Pipelines;

public class ReadAsyncCompletionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Test1(bool isEncrypt)
    {
        async Task Await(ValueTask<ReadResult> a)
        {
            await a;
        }

        var pipe = new Pipe(
            new PipeOptions(
                useSynchronizationContext: false,
                writerScheduler: PipeScheduler.Inline, 
                readerScheduler: PipeScheduler.Inline,
                resumeWriterThreshold: 6,
                pauseWriterThreshold: 65));

        var reader = new CryptoPipeReader(pipe.Reader);

        if (isEncrypt)
        {
            reader.EnableEncryption(Crypto.CreateDecryptor());
        }
        
        ValueTask<ReadResult> awaitable1 = reader.ReadAsync();
        //ValueTask<ReadResult> awaitable2 = reader.ReadAsync();
    
        Task task1 = Await(awaitable1);
        Task task2 = Await(awaitable1);
        Thread.Sleep(500);
        Assert.True(task1.IsCompleted, $"{task1.Status}");
        Assert.True(task1.IsFaulted);
        Assert.Equal("Concurrent reads or writes are not supported.", task1.Exception.InnerExceptions[0].Message);

        Assert.True(task2.IsCompleted);
        Assert.True(task2.IsFaulted);
        Assert.Equal("Concurrent reads or writes are not supported.", task2.Exception.InnerExceptions[0].Message);
    }
    
    
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CompletingWithExceptionDoesNotAffectState(bool isEncrypt)
    {
        var pipe = new Pipe(
            new PipeOptions(
                useSynchronizationContext: false,
                writerScheduler: PipeScheduler.Inline, 
                readerScheduler: PipeScheduler.Inline,
                resumeWriterThreshold: 6,
                pauseWriterThreshold: 65));

        var reader = new CryptoPipeReader(pipe.Reader);

        if (isEncrypt)
        {
            reader.EnableEncryption(Crypto.CreateDecryptor());
        }

        var writer = new CryptoPipeWriter(pipe.Writer);

        if (isEncrypt)
        {
           writer.EnableEncryption(Crypto.CreateEncryptor());
        }
        
        reader.Complete();
        reader.Complete(new Exception());

        var result = await writer.FlushAsync();
        Assert.True(result.IsCompleted);
    }

}