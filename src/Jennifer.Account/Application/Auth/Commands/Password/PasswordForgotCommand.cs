using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.Password;

//PasswordForgot은 비로그인 상태에서 암호변경

public sealed record PasswordForgotRequest(string Email, string UserName);
public sealed record PasswordForgotCommand(string Email, string UserName):ICommand<Result>;



