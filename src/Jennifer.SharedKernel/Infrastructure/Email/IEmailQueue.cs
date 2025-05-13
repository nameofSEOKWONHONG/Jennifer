namespace Jennifer.SharedKernel.Infrastructure.Email;

public interface IEmailQueue
{
    ValueTask EnqueueAsync(EmailMessage email, CancellationToken cancellationToken = default);
    ValueTask<EmailMessage> DequeueAsync(CancellationToken cancellationToken);
}