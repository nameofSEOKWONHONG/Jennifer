namespace Jennifer.SharedKernel.Infrastructure.Email;

using System.Threading.Channels;

/// <summary>
/// Represents a thread-safe queue for managing email messages in a producer-consumer model.
/// EmailQueue is designed to handle the enqueuing and dequeuing of email messages using
/// a bounded channel for efficient and safe message processing.
/// </summary>
/// <remarks>
/// This class enables efficient handling of email processing workloads by using a bounded
/// channel with configurable capacity to restrict resource usage and avoid overloading
/// the system. The queue can be used with one reader and multiple writers. It is particularly
/// useful in scenarios where asynchronous message passing is required between components
/// of an application.
/// </remarks>
public class EmailQueue : IEmailQueue
{
    private readonly Channel<EmailMessage> _channel;

    public EmailQueue()
    {
        // BoundedChannelOption을 사용해 제한된 버퍼 설정 가능
        var options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };
        _channel = Channel.CreateBounded<EmailMessage>(options);
    }

    public async ValueTask EnqueueAsync(EmailMessage email, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(email, cancellationToken);
    }

    public async ValueTask<EmailMessage> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }
}
