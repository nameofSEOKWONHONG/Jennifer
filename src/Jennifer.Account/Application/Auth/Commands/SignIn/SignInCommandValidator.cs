﻿using FluentValidation;

namespace Jennifer.Account.Application.Auth.Commands.SignIn;

public sealed class SignInCommandValidator: AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
        RuleFor(m => m.Password).NotEmpty();
    }
}