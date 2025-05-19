using Jennifer.Jwt.Infrastructure.Email;

namespace Jennifer.SharedKernel.Infrastructure.Email;

/// <summary>
/// Defines a contract for a queue system responsible for managing email messages
/// in a producer-consumer model. The queue provides methods for enqueuing and
/// dequeuing email messages in an asynchronous and thread-safe manner.
/// </summary>
/// <remarks>
/// Implementations of this interface should enable the addition of email messages
/// into the queue and their subsequent retrieval for processing. It is suitable
/// for scenarios that require asynchronous email processing.
/// </remarks>
public interface IEmailQueue
{
    ValueTask EnqueueAsync(EmailMessage email, CancellationToken cancellationToken = default);
    ValueTask<EmailMessage> DequeueAsync(CancellationToken cancellationToken);
}