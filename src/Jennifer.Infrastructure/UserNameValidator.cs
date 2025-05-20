using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Infrastructure;

public class UserNameValidator<TUser> : IUserValidator<TUser>
    where TUser : class
{
    private static readonly Regex _regex = new(@"^[가-힣a-zA-Z0-9]+$", RegexOptions.Compiled);

    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
    {
        var errors = new List<IdentityError>();

        var userNameProp = typeof(TUser).GetProperty("UserName");
        var userName = userNameProp?.GetValue(user) as string;

        if (string.IsNullOrWhiteSpace(userName) || !_regex.IsMatch(userName))
        {
            errors.Add(new IdentityError
            {
                Code = "InvalidUserName",
                Description = "사용자 이름은 한글과 숫자만 포함할 수 있습니다."
            });
        }

        return Task.FromResult(errors.Count == 0
            ? IdentityResult.Success
            : IdentityResult.Failed(errors.ToArray()));
    }
}