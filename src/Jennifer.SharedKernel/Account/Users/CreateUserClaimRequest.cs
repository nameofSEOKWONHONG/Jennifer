using Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jennifer.SharedKernel.Account.Users;

public sealed record CreateUserClaimRequest(string ClaimType, string ClaimValue);
public sealed record ModifyUserRequest(Guid UserId, string UserName, string PhoneNumber);
public class GetsUserRequest : PagingRequest
{
    public string Email { get; set; }
    public string UserName { get; set; }
}