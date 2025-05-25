using FluentValidation;
using Jennifer.Account.Models;
using Jennifer.SharedKernel;

namespace Jennifer.Account.Application.Auth.Contracts;

public class UserPagingRequest : PagingRequest
{
    public string Email { get; set; }    
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsDelete { get; set; }
    public IEnumerable<string> RoleNames { get; set; }
    public IEnumerable<UserClaimDto> UserClaims { get; set; }   
}

public class UserClaimDto
{
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }   
}

public class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(m => m.Id).NotEmpty();
        RuleFor(m => m.Email).NotEmpty()
            .EmailAddress()
            .MaximumLength(255);
        RuleFor(m => m.UserName).NotEmpty()
            .MaximumLength(255)
            .MinimumLength(2)
            .Matches(@"^[a-zA-Z0-9]+$")
            ;
        RuleFor(m => m.PhoneNumber).NotEmpty()
            .MaximumLength(20)
            .MinimumLength(10);
    }
}
