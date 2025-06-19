using Jennifer.Domain.Accounts;
using Mediator;

namespace Jennifer.Account.Application.Users.Commands;

public sealed class UserWithdrawDomainEventHandler() : INotificationHandler<UserWithdrawDomainEvent>
{
    public ValueTask Handle(UserWithdrawDomainEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        
        //TODO: 메일 통지.
        //TODO: 삭제 관련 서비스 예약
        /*
         * public virtual ICollection<UserRole> UserRoles { get; set; }
           public virtual ICollection<UserClaim> UserClaims { get; set; }
           public virtual ICollection<UserLogin> Logins { get; set; }
           public virtual ICollection<UserToken> Tokens { get; set; }
           public virtual ICollection<UserOption> UserOptions { get; set; }
         */
    }
}