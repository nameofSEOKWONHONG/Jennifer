using FluentValidation;
using Jennifer.SharedKernel;

namespace Jennifer.Infrastructure.Abstractions.ServiceCore;

public abstract class ServiceBase<TRequest, TResult> : IServiceBase<TRequest, TResult>
where TResult : IResult
{
    private readonly IValidator<TRequest> _validator;

    protected ServiceBase(IValidator<TRequest> validator = null)
    {
        _validator = validator;
    }

    public async Task<TResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(request, cancellationToken);
        await OnPreExecuteAsync(request, cancellationToken);
        return await HandleAsync(request, cancellationToken);
    }

    protected virtual Task OnPreExecuteAsync(TRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

    protected abstract Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken);

    private async Task ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (_validator != null)
        {
            var result = await _validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }
        else if (typeof(TRequest).IsClass)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(typeof(TRequest));
            var validator = _validator ?? (IValidator<TRequest>)_provider.GetService(validatorType);
            if (validator != null)
            {
                var result = await validator.ValidateAsync(request, cancellationToken);
                if (!result.IsValid)
                    throw new ValidationException(result.Errors);
            }
        }
    }

    private IServiceProvider _provider;
    public void SetServiceProvider(IServiceProvider provider) => _provider = provider;
}