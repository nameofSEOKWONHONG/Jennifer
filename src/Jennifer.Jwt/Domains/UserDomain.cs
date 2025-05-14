using FluentValidation;
using Jennifer.SharedKernel.Infrastructure;

namespace Jennifer.Jwt.Domains;

public class UserPagingRequest : PagingRequest
{
    public string Email { get; set; }    
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}

public class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(m => m.Id).NotEmpty();
        RuleFor(m => m.Email).NotEmpty()
            .EmailAddress()
            .MaximumLength(255);
        RuleFor(m => m.Name).NotEmpty()
            .MaximumLength(255)
            .MinimumLength(2)
            .Matches(@"^[a-zA-Z0-9]+$")
            ;
        RuleFor(m => m.PhoneNumber).NotEmpty()
            .MaximumLength(20)
            .MinimumLength(10);
        RuleFor(m => m.Password).NotEmpty()
            .MaximumLength(255)
            .MinimumLength(8);
    }
}

