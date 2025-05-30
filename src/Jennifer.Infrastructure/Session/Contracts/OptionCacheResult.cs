using Jennifer.Domain.Accounts.Contracts;

namespace Jennifer.Infrastructure.Session.Contracts;

public class OptionCacheResult
{
    public ENUM_OPTION_TYPE Type { get; set; }
    public string Value { get; set; }
}