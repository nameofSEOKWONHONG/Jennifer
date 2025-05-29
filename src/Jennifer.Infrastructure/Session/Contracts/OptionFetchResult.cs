using Jennifer.Domain.Account.Contracts;

namespace Jennifer.Infrastructure.Session.Contracts;

public class OptionFetchResult
{
    public ENUM_OPTION_TYPE Type { get; set; }
    public string Value { get; set; }
}