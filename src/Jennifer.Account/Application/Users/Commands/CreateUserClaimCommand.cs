using FluentValidation;
using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Account.Application.Users.Commands;

public sealed record CreateUserClaimRequest(string ClaimType, string ClaimValue);
public sealed record CreateUserClaimCommand(Guid UserId, CreateUserClaimRequest[] requests) : ICommand<Result>;

public sealed class CreateUserClaimCommandHandler(JenniferDbContext dbContext): ICommandHandler<CreateUserClaimCommand, Result>
{
    public async ValueTask<Result> Handle(CreateUserClaimCommand command, CancellationToken cancellationToken)
    {
        await dbContext.UserClaims.Where(m => m.UserId == command.UserId)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        
        var list = new List<UserClaim>();
        foreach (var createUserClaimRequest in command.requests)
        {
            list.Add(new UserClaim()
            {
                UserId = command.UserId,
                ClaimType = createUserClaimRequest.ClaimType,
                ClaimValue = createUserClaimRequest.ClaimValue,
            });
        }
        await dbContext.UserClaims.AddRangeAsync(list, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result.SuccessAsync();
    }
}

public sealed class CreateUserClaimCommandValidator : AbstractValidator<CreateUserClaimCommand>
{
    public CreateUserClaimCommandValidator(IValidator<CreateUserClaimRequest> requestValidator)
    {
        RuleFor(m => m.UserId).NotEmpty();
        RuleFor(m => m.requests)
            .NotNull()
            .Must(r => r.Length > 0);
        RuleForEach(m => m.requests)
            .SetValidator(requestValidator);
    }
}

/// <summary>
/// TODO: DB 로 타입을 체크하는 예... 필요하면 캐시화
/// </summary>
public sealed class CreateUserClaimRequestValidator : AbstractValidator<CreateUserClaimRequest>
{
    public CreateUserClaimRequestValidator(IServiceProvider serviceProvider)
    {
        RuleFor(x => x.ClaimType)
            .NotEmpty()
            .MustAsync(async (type, ct) =>
            {
                var db = serviceProvider.GetRequiredService<JenniferDbContext>();
                var exists = await db.Options.AsNoTracking().AnyAsync(m => m.Type == type, ct);
                return exists;
            });

        RuleFor(x => x.ClaimValue)
            .NotEmpty()
            .MustAsync(async (value, ct) =>
            {
                var db = serviceProvider.GetRequiredService<JenniferDbContext>();
                var exists = await db.Options.AsNoTracking().AnyAsync(m => m.Value == value, ct);
                return exists;
            });

        RuleFor(m => m)
            .MustAsync(async (item, ct) =>
            {
                var db = serviceProvider.GetRequiredService<JenniferDbContext>();
                var exists = await db.UserClaims.AsNoTracking()
                    .AnyAsync(m => m.ClaimType == item.ClaimType && m.ClaimValue == item.ClaimValue, ct);
                return !exists;
            });
    }
}