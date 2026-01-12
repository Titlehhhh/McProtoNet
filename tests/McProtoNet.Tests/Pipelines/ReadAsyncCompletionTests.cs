using System.IO.Pipelines;
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

        var reader = new DecryptedPipeReader(pipe.Reader);

        if (isEncrypt)
        {
            reader.SwitchEncryption(Crypto.CreateCFB(false));
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

        var reader = new DecryptedPipeReader(pipe.Reader);

        if (isEncrypt)
        {
            reader.SwitchEncryption(Crypto.CreateCFB(false));
        }

        var writer = new EncryptedPipeWriter(pipe.Writer);

        if (isEncrypt)
        {
           writer.SwitchEncryption(Crypto.CreateCFB(true)); 
        }
        
        reader.Complete();
        reader.Complete(new Exception());

        var result = await writer.FlushAsync();
        Assert.True(result.IsCompleted);
    }

}