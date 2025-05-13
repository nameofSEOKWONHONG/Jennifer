using Jennifer.Core.Infrastructure;

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