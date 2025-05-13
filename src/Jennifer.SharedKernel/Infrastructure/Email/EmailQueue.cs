namespace Jennifer.SharedKernel.Infrastructure.Email;

using System.Threading.Channels;

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
