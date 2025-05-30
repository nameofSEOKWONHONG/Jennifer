using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.Password;

//로그인 된 상태에서 암호 변경을 의미.

public sealed record PasswordChangeCommand(string OldPassword, string NewPassword):ICommand<Result<bool>>;


