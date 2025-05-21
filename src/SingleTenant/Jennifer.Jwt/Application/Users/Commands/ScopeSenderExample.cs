using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Jwt.Application.Users.Commands;

public sealed record ExampleQuery(string Type): IQuery<Result<string>>;
public class  Example1QueryHandler : IQueryHandler<ExampleQuery, Result<string>>
{
    public async ValueTask<Result<string>> Handle(ExampleQuery query, CancellationToken cancellationToken)
    {
        return await Task.FromResult(Result<string>.Success("1"));
    }
}

public class  Example2QueryHandler : IQueryHandler<ExampleQuery, Result<string>>
{
    public async ValueTask<Result<string>> Handle(ExampleQuery query, CancellationToken cancellationToken)
    {
        return await Task.FromResult(Result<string>.Success("1"));
    }
}

public class  Example3QueryHandler : IQueryHandler<ExampleQuery, Result<string>>
{
    public async ValueTask<Result<string>> Handle(ExampleQuery query, CancellationToken cancellationToken)
    {
        return await Task.FromResult(Result<string>.Success("1"));
    }
}