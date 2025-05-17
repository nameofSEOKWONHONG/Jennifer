using eXtensionSharp;
using FluentValidation;
using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.AuthServices.Implements;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static Error Unauthorized() => Error.Failure(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");

    public static readonly Error NotFoundByEmail = Error.NotFound(
        "Users.NotFoundByEmail",
        "The user with the specified email was not found");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "Users.EmailNotUnique",
        "The provided email is not unique");
}

    









public sealed record DeleteUserCommand(Guid UserId) : ICommand;

internal class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    public Task<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        // 처리 로직
        return Task.FromResult(Result.Success());
    }
}

public sealed record UserResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }
}


public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;

internal sealed class GetUserByEmailQueryHandler()
    : IQueryHandler<GetUserByEmailQuery, UserResponse>
{
    public Task<Result<UserResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        return null;
    }
}

