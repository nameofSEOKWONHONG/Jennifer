namespace Jennifer.Infrastructure.Abstractions.ServiceCore;

public interface IExecutionUnit
{
    bool CanExecute();
    Task<object> ExecuteAsync(CancellationToken cancellationToken);
    void ApplyResult(object result);
}